using System.Numerics;

namespace SharpRay
{
    public interface IEvent { }

    public interface IKeyBoardEvent { }

    public interface IMouseEvent { public Vector2 Position { get; init; } }

    public interface IUIEvent { UIComponent UIComponent { get; init; } }

    public interface IAudioEvent { }

    public interface ICollisionEvent { Entity Entity { get; init; } }

    public struct CollisionEvent : ICollisionEvent { public Entity Entity { get; init; } }

}
