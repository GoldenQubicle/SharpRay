namespace SharpRay.Core
{
    public record Config
    {
        public static double TickMultiplier = 10000d;
        public int WindowWidth { get; init; }
        public int WindowHeight { get; init; }
    }
}