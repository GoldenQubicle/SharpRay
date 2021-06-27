using System;
using System.Numerics;

namespace SharpRay
{
    public interface IEvent { }

    public interface IEventEmitter<TEvent> where TEvent : IEvent { Action<TEvent> EmitEvent { get; set; } }

    public interface IKeyBoardEvent : IEvent { }

    public interface IMouseEvent : IEvent { public Vector2 Position { get; init; } }

    public interface IUIEvent : IEvent { UIEntity UIComponent { get; init; } }

    public interface IGameEvent : IEvent { }

}
