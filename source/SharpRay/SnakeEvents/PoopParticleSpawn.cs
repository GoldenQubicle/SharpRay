using System.Numerics;

namespace SharpRay
{
    public struct PoopParticleSpawn : IGameEvent 
    { 
        public Vector2 Position { get; init; }
    }
}
