using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExtension
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    internal class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        private readonly Dictionary<ParameterExpression, Expression> replaceVars;
        internal ExpressionExpander() { }

        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
        {
            this.replaceVars = replaceVars;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return (replaceVars != null) && (replaceVars.ContainsKey(p)) ? replaceVars[p] : base.VisitParameter(p);
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            var target = iv.Expression;
            if (target is MemberExpression)
            {
                target = TransformExpr((MemberExpression)target);
            }
            if (target is ConstantExpression)
            {
                target = ((ConstantExpression)target).Value as Expression;
            }

            var lambda = (LambdaExpression)target;

            var expressions = replaceVars == null
                ? new Dictionary<ParameterExpression, Expression>()
                : new Dictionary<ParameterExpression, Expression>(replaceVars);

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    expressions.Add(lambda.Parameters[i], Visit(iv.Arguments[i]));
                }
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    "Invoke cannot be called recursively - try using a temporary variable.", ex);
            }

            return new ExpressionExpander(expressions).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(Extensions))
            {
                var target = m.Arguments[0];
                if (target is MemberExpression)
                {
                    target = TransformExpr((MemberExpression)target);
                }
                if (target is ConstantExpression)
                {
                    target = ((ConstantExpression)target).Value as Expression;
                }
                if (target is UnaryExpression)
                {
                    target = ((UnaryExpression)target).Operand as Expression;
                }

                var lambda = (LambdaExpression)target;

                if (lambda != null)
                {
                    var expressions = replaceVars == null
                        ? new Dictionary<ParameterExpression, Expression>()
                        : new Dictionary<ParameterExpression, Expression>(replaceVars);

                    try
                    {
                        for (int i = 0; i < lambda.Parameters.Count; i++)
                        {
                            expressions.Add(lambda.Parameters[i], Visit(m.Arguments[i + 1]));
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidOperationException(
                            "Invoke cannot be called recursively - try using a temporary variable.", ex);
                    }

                    return new ExpressionExpander(expressions).Visit(lambda.Body);
                }
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name == "Compile" && m.Object is MemberExpression)
            {
                var me = (MemberExpression)m.Object;
                var newExpr = TransformExpr(me);
                if (newExpr != me)
                {
                    return newExpr;
                }
            }

            // Strip out any nested calls to AsExpandable():
            if (m.Method.Name == "AsExpandable" && m.Method.DeclaringType == typeof(Extensions))
            {
                return m.Arguments[0];
            }

            return base.VisitMethodCall(m);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            // Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
            return m.Member.DeclaringType != null && m.Member.DeclaringType.Name.StartsWith("<>")
                ? TransformExpr(m)
                : base.VisitMemberAccess(m);
        }

        private Expression TransformExpr(MemberExpression input)
        {
            if (input == null)
            {
                return null;
            }

            var field = input.Member as FieldInfo;

            if (field == null)
            {
                if ((replaceVars != null)
                    && (input.Expression is ParameterExpression)
                    && (replaceVars.ContainsKey((ParameterExpression)input.Expression)))
                {
                    return base.VisitMemberAccess(input);
                }
                else
                {
                    return input;
                }
            }

            // Collapse captured outer variables
            if (input.Member.ReflectedType != null
                && (!input.Member.ReflectedType.IsNestedPrivate || !input.Member.ReflectedType.Name.StartsWith("<>")))
            // captured outer variable
            {
                return TryVisitExpressionFunc(input, field);
            }

            var expression = input.Expression as ConstantExpression;
            if (expression != null)
            {
                var obj = expression.Value;
                if (obj == null)
                {
                    return input;
                }
                var t = obj.GetType();
                if (!t.IsNestedPrivate || !t.Name.StartsWith("<>"))
                {
                    return input;
                }
                var fi = (FieldInfo)input.Member;
                var result = fi.GetValue(obj);
                var exp = result as Expression;
                if (exp != null)
                {
                    return Visit(exp);
                }
            }

            return TryVisitExpressionFunc(input, field);
        }

        private Expression TryVisitExpressionFunc(MemberExpression input, FieldInfo field)
        {
            var prope = input.Member as PropertyInfo;
            if ((field.FieldType.IsSubclassOf(typeof(Expression))) ||
                (prope != null && prope.PropertyType.IsSubclassOf(typeof(Expression))))
            {
                return Visit(Expression.Lambda<Func<Expression>>(input).Compile()());
            }

            return input;
        }
    }
}