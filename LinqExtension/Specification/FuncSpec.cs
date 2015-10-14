using System;

namespace LinqExtension
{
    /// <summary>
    /// ��Լʵ����
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
        /// Լ�������Ƿ������Լ
        /// </summary>
        /// <param name="candidate">Լ������</param>
        public bool IsSatisfiedBy(T candidate)
        {
            return isSatisfiedBy(candidate);
        }
    }
}