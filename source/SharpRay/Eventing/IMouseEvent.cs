using System.Numerics;

namespace SharpRay.Eventing
{
    public interface IMouseEvent : IEvent { public Vector2 Position { get; init; } }

}
