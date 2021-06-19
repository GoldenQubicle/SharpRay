namespace SharpRay
{
    public struct SnakeConsumedFood : IGameEvent
    {
        public ParticleFood FoodParticle { get; init; }
        public Segment NextSegment { get; init; }
        public int SnakeLength { get; init; }
    }
}
