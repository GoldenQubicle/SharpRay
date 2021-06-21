namespace SharpRay
{
    public struct SnakeConsumedPoop : IGameEvent 
    { 
        public Segment Tail { get; init; }
        public ParticlePoop PoopParticle { get; init; }
    }

}
