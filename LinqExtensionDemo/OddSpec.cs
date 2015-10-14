namespace LinqExtension
{
    public class OddSpec : ISpec<int>
    {
        public bool IsSatisfiedBy(int candidate)
        {
            return candidate % 2 != 0;
        }
    }
}