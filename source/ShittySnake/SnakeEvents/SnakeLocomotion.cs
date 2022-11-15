using SharpRay.Eventing;
using SnakeEntities;
using System.Numerics;

namespace SnakeEvents
{
    public struct SnakeLocomotion : IGameEvent
    {
        public Direction Direction { get; init; }
        public Vector2 Position { get; init; }
    }
}
