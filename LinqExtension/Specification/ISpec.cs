namespace LinqExtension
{
    /// <summary>
    /// ��Լ�ӿ�
    /// </summary>
    public interface ISpec<T>
    {
        /// <summary>
        /// Լ�������Ƿ������Լ
        /// </summary>
        /// <param name="candidate">Լ������</param>
        bool IsSatisfiedBy(T candidate);
    }
}