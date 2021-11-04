namespace SharpRay.Core
{
    public record Config
    {
        public static readonly double TickMultiplier = 10000d;
        public int WindowWidth { get; init; }
        public int WindowHeight { get; init; }
        public bool DoEventLogging { get; init; }
    }
}