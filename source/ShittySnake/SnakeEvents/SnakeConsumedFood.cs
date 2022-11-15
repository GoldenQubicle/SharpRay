using SharpRay.Eventing;
using SnakeEntities;

namespace SnakeEvents
{
    public struct SnakeConsumedFood : IGameEvent
    {
        public ParticleFood FoodParticle { get; init; }
        public Segment NextSegment { get; init; }
        public int SnakeLength { get; init; }
    }
}
