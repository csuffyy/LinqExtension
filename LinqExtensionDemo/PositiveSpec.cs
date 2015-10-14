namespace LinqExtension
{
    public class PositiveSpec : ISpec<int>
    {
        public bool IsSatisfiedBy(int candidate)
        {
            return candidate > 0;
        }
    }
}