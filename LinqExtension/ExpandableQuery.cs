using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace LinqExtension
{
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
    public sealed class ExpandableQuery<T> : IOrderedQueryable<T>
    {
        private readonly ExpandableQueryProvider<T> provider;

        internal ExpandableQuery(IQueryable<T> inner)
        {
            InnerQuery = inner;
            provider = new ExpandableQueryProvider<T>(this);
        }

        // Original query, that we're wrapping
        internal IQueryable<T> InnerQuery { get; }

        Expression IQueryable.Expression => InnerQuery.Expression;

        Type IQueryable.ElementType => typeof (T);

        IQueryProvider IQueryable.Provider => provider;

        /// <summary> IQueryable enumeration </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        /// <summary> IQueryable string presentation.  </summary>
        public override string ToString()
        {
            return InnerQuery.ToString();
        }
    }
}