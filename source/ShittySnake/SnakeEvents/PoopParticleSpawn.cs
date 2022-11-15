using System.Numerics;
using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct PoopParticleSpawn : IGameEvent 
    { 
        public Vector2 Position { get; init; }
    }
}
