using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtension
{
    /// <summary>
    /// 规约委托
    /// </summary>
    /// <typeparam name="T">实体类型约束</typeparam>
    /// <param name="candidate">约束对象</param>
    /// <returns>约束对象是否符合规约</returns>
    public delegate bool Spec<in T>(T candidate);

    /// <summary>
    /// 规约接口
    /// </summary>
    public interface ISpec<in T>
    {
        /// <summary>
        /// 约束对象是否满足规约
        /// </summary>
        /// <param name="candidate">约束对象</param>
        bool IsSatisfiedBy(T candidate);
    }

    /// <summary>
    /// 规约实现类
    /// </summary>
    public class Specification<T> : ISpec<T>
    {
        private readonly Func<T, bool> isSatisfiedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{T}"/> class.
        /// </summary>
        /// <param name="isSatisfiedBy">The is satisfied by.</param>
        public Specification(Func<T, bool> isSatisfiedBy)
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

    /// <summary>
    /// 规约扩展类
    /// </summary>
    public static class SpecExtensitions
    {
        #region Spec<T>

        /// <summary>
        /// 与
        /// </summary>
        public static Spec<T> Add<T>(this Spec<T> one, Spec<T> other)
        {
            return candidate => one(candidate) && other(candidate);
        }

        /// <summary>
        /// 或
        /// </summary>
        public static Spec<T> Or<T>(this Spec<T> one, Spec<T> other)
        {
            return candidate => one(candidate) || other(candidate);
        }

        /// <summary>
        /// 非
        /// </summary>
        public static Spec<T> Not<T>(this Spec<T> one)
        {
            return candidate => !one(candidate);
        }

        #endregion

        #region Func<T, bool>

        /// <summary>
        /// 与
        /// </summary>
        public static Func<T, bool> Add<T>(this Func<T, bool> one, Func<T, bool> other)
        {
            return candidate => one(candidate) && other(candidate);
        }

        /// <summary>
        /// 或
        /// </summary>
        public static Func<T, bool> Or<T>(this Func<T, bool> one, Func<T, bool> other)
        {
            return candidate => one(candidate) || other(candidate);
        }

        /// <summary>
        /// 非
        /// </summary>
        public static Func<T, bool> Not<T>(this Func<T, bool> one)
        {
            return candidate => !one(candidate);
        }

        #endregion

        #region ISpec

        public static ISpec<T> Add<T>(this ISpec<T> one, ISpec<T> other)
        {
            return new Specification<T>(candidate => one.IsSatisfiedBy(candidate) && other.IsSatisfiedBy(candidate));
        }

        public static ISpec<T> Or<T>(this ISpec<T> one, ISpec<T> other)
        {
            return new Specification<T>(candidate => one.IsSatisfiedBy(candidate) || other.IsSatisfiedBy(candidate));
        }

        public static ISpec<T> Not<T>(this ISpec<T> one)
        {
            return new Specification<T>(candidate => !one.IsSatisfiedBy(candidate));
        }

        #endregion
    }
}