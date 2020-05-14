namespace RulesBot.Core
{
    public class Railway<T>
    {
        public T Value { get; }
        public bool Handled { get; set; }
        public Railway(T value)
        {
            Value = value;
        }
    }

    public static class RailwayExtensions
    {
        public static Railway<T> AsRailway<T>(this T source) => new Railway<T>(source);
    }
}
