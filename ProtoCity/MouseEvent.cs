using System;
using System.Numerics;

namespace ProtoCity
{
    public interface IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseLeftClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseMiddleClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseRighttClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseLeftDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseMiddleDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseRightDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseLeftRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseMiddleRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseRightRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseWheelUp : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }

    public struct MouseWheelDown : IMouseEvent
    {
        public Vector2 Position { get; init; }
    }
}
