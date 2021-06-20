using System.Numerics;

namespace SharpRay
{
    public struct SnakeLocomotion : IGameEvent
    {
        public Direction Direction { get; init; }
        public Vector2 Position { get; init; }
    }
}
