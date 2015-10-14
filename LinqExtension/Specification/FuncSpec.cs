using System;

namespace LinqExtension
{
    /// <summary>
    /// 规约实现类
    /// </summary>
    public class FuncSpec<T> : ISpec<T>
    {
        private readonly Func<T, bool> isSatisfiedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncSpec{T}"/> class.
        /// </summary>
        /// <param name="isSatisfiedBy">The is satisfied by.</param>
        public FuncSpec(Func<T, bool> isSatisfiedBy)
        {
            this.isSatisfiedBy = isSatisfiedBy;
        }

        /// <summary>
        /// 约束对象是否满足规约
        /// </summary>
        /// <param name="candidate">约束对象</param>
        public bool IsSatisfiedBy(T candidate)
        {
            return isSatisfiedBy(candidate);
        }
    }
}