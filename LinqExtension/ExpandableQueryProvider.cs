using System.Linq;
using System.Linq.Expressions;

namespace LinqExtension
{
    class ExpandableQueryProvider<T> : IQueryProvider
    {
        readonly ExpandableQuery<T> query;

        internal ExpandableQueryProvider(ExpandableQuery<T> query)
        {
            this.query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return query.InnerQuery.Provider.CreateQuery<TElement>(expression.Expand()).AsExpandable();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return query.InnerQuery.Provider.CreateQuery(expression.Expand());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return query.InnerQuery.Provider.Execute<TResult>(expression.Expand());
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return query.InnerQuery.Provider.Execute(expression.Expand());
        }
    }
}