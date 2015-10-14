namespace LinqExtension
{
    /// <summary>
    /// 规约接口
    /// </summary>
    public interface ISpec<T>
    {
        /// <summary>
        /// 约束对象是否满足规约
        /// </summary>
        /// <param name="candidate">约束对象</param>
        bool IsSatisfiedBy(T candidate);
    }
}