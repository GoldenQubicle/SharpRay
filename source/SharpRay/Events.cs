using System.Numerics;

namespace SharpRay
{
    public interface IEvent { }

    public interface IKeyBoardEvent : IEvent { }

    public interface IMouseEvent : IEvent { public Vector2 Position { get; init; } }

    public interface IUIEvent : IEvent { UIComponent UIComponent { get; init; } }
   
    public interface IAudioEvent : IEvent { }
}
