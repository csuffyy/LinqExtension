using System;

namespace LinqExtension
{
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
            return new FuncSpec<T>(candidate => one.IsSatisfiedBy(candidate) && other.IsSatisfiedBy(candidate));
        }

        public static ISpec<T> Or<T>(this ISpec<T> one, ISpec<T> other)
        {
            return new FuncSpec<T>(candidate => one.IsSatisfiedBy(candidate) || other.IsSatisfiedBy(candidate));
        }

        public static ISpec<T> Not<T>(this ISpec<T> one)
        {
            return new FuncSpec<T>(candidate => !one.IsSatisfiedBy(candidate));
        }

        #endregion
    }
}