using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExtension
{
    public static class LinqExtensions
    {
        private static Expression<Func<T, bool>> BuildPredicate<T>(string member, object value)
        {
            var column = member.ToUpper();
            var verif = false;
            var propertyTypeName = string.Empty;

            foreach (var type in typeof(T).GetProperties())
            {
                if (type.ToString().ToUpper().Contains(column.ToUpper()))
                {
                    verif = true;
                    var property = type;
                    if (property.PropertyType.Name.Contains("Nullable"))
                    {
                        if (property.PropertyType.ToString().Contains("DateTime"))
                        {
                            propertyTypeName = "DateTime";
                        }
                        else
                        {
                            if (property.PropertyType.ToString().Contains("Int32"))
                            {
                                propertyTypeName = "Int32";
                            }
                            else
                            {
                                propertyTypeName = "Double";
                            }
                        }
                    }
                    else
                    {
                        propertyTypeName = property.PropertyType.Name;
                    }
                    break;
                }
            }

            if (verif)
            {
                var p = Expression.Parameter(typeof(T), "entity");
                Expression body = p;
                body = Expression.PropertyOrField(body, member);
                switch (propertyTypeName)
                {
                    case "Int32":
                        {
                            #region traitement des entiers

                            var operations = new[] { ">=", "<=", "<", ">", "=" };
                            var operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "String":
                        {
                            #region traitement des string

                            var operation = "Contains";
                            if (value.ToString().StartsWith("%") && !value.ToString().EndsWith("%"))
                            {
                                operation = "StartsWith";
                            }
                            else
                            {
                                if (!value.ToString().StartsWith("%") && value.ToString().EndsWith("%"))
                                {
                                    operation = "EndsWith";
                                }
                                else
                                {
                                    if (value.ToString().StartsWith("="))
                                    {
                                        value = value.ToString().Replace("=", "").TrimStart();
                                    }
                                }
                            }
                            value = value.ToString().Replace("%", "").Trim();

                            Expression Out = Expression.Call(
                                                Expression.Call( // <=== this one is new
                                                    body,
                                                    "ToUpper", null),
                                                operation, null,   //  Param_0 => Param_0.FirstName.ToUpper().Contains("MYVALUE")
                                                Expression.Constant(value.ToString().ToUpper()));

                            return Expression.Lambda<Func<T, bool>>(Out, p);

                            #endregion
                        }
                    case "Decimal":
                        {
                            #region traitement des decimales

                            var operations = new[] { ">=", "<=", "<", ">", "=" };
                            var operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "DateTime":
                        {
                            #region traitement des dates

                            var operations = new[] { ">=", "<=", "<", ">", "=" };
                            var operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            DateTime objDate;
                            if (string.IsNullOrEmpty(value.ToString()))
                            {
                                var dtfi = new DateTimeFormatInfo
                                {
                                    ShortDatePattern = "dd/MM/yyyy",
                                    DateSeparator = "/"
                                };
                                objDate = Convert.ToDateTime("1/1/1900", dtfi);
                                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                            }
                            else
                            {
                                var dtfi = new DateTimeFormatInfo
                                {
                                    ShortDatePattern = "dd/MM/yyyy",
                                    DateSeparator = "/"
                                };
                                objDate = Convert.ToDateTime(value, dtfi);
                            }

                            switch (operation)
                            {
                                case ">=":
                                    {

                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "DateTimeOffset":
                        {
                            #region traitement des dates

                            var operations = new[] { ">=", "<=", "<", ">", "=" };
                            var operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            DateTimeOffset objDate;
                            if (string.IsNullOrEmpty(value.ToString()))
                            {
                                var dtfi = new DateTimeFormatInfo
                                {
                                    ShortDatePattern = "dd/MM/yyyy",
                                    DateSeparator = "/"
                                };
                                objDate = Convert.ToDateTime("1/1/1900", dtfi);
                                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                            }
                            else
                            {
                                var dtfi = new DateTimeFormatInfo
                                {
                                    ShortDatePattern = "dd/MM/yyyy",
                                    DateSeparator = "/"
                                };
                                objDate = Convert.ToDateTime(value, dtfi);
                            }

                            switch (operation)
                            {
                                case ">=":
                                    {

                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "Double":
                        {
                            #region traitement les Double


                            var operations = new[] { ">=", "<=", "<", ">", "=" };
                            var operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<T> Query<T>(this IEnumerable<T> list, Dictionary<string, List<string>> dictionary)
        {
            var tpredicate = PredicateBuilder.True<T>();
            foreach (var data in dictionary)
            {
                var key = data.Key;
                var valeur = data.Value;
                var subpredicate = PredicateBuilder.False<T>();
                foreach (var val in valeur)
                {
                    subpredicate = subpredicate.Or(BuildPredicate<T>(key, val));
                }
                tpredicate = tpredicate.And(subpredicate.Expand());  // OK at runtime!
            }

            var predicate = tpredicate.Compile();
            return list.Where(predicate);
        }

        public static IQueryable<T> Query<T>(this IQueryable<T> entity, Dictionary<string, List<string>> dictionary)
        {
            var predicate = PredicateBuilder.True<T>();
            foreach (var data in dictionary)
            {
                var key = data.Key;
                var valeur = data.Value;
                var subpredicate = PredicateBuilder.False<T>();
                foreach (var val in valeur)
                {
                    subpredicate = subpredicate.Or(BuildPredicate<T>(key, val));
                }
                predicate = predicate.And(subpredicate.Expand());  // OK at runtime!
            }
            return entity.AsExpandable().Where(predicate);
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> list, bool condition, Func<T, bool> predicate)
        {
            if (condition)
            {
                return list.Where(predicate);
            }
            else
            {
                return list;
            }
        }
    }
}
