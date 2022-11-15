using SharpRay.Eventing;
using SnakeEntities;

namespace SnakeEvents
{
    public struct DespawnPoop : IGameEvent 
    { 
        public ParticlePoop PoopParticle { get; init; }
    }

}
