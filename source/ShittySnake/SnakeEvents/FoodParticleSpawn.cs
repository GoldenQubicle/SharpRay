using System.Numerics;
using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct FoodParticleSpawn : IGameEvent { public Vector2 Position { get; init; } }
}
