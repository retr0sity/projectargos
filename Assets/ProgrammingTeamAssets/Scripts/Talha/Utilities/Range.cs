namespace Game.Utilities
{
    [System.Serializable]
    public class Range<T>
    {
        public T Min;
        public T Max;

        public Range(T Min,T Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }
}