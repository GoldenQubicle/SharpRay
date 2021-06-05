using System;
using System.Numerics;

namespace ProtoCity
{
    public interface IMouseEvent
    {
        public Vector2 Position { get; }
    }

    public struct MouseLeftClick : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseLeftClick(Vector2 position) => Position = position;
    }

    public struct MouseMiddleClick : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseMiddleClick(Vector2 position) => Position = position;
    }

    public struct MouseRighttClick : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseRighttClick(Vector2 position) => Position = position;
    }

    public struct MouseLeftDrag : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseLeftDrag(Vector2 position) => Position = position;
    }

    public struct MouseMiddleDrag : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseMiddleDrag(Vector2 position) => Position = position;
    }

    public struct MouseRightDrag : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseRightDrag(Vector2 position) => Position = position;
    }

    public struct MouseLeftRelease : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseLeftRelease(Vector2 position) => Position = position;
    }

    public struct MouseMiddleRelease : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseMiddleRelease(Vector2 position) => Position = position;
    }

    public struct MouseRightRelease : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseRightRelease(Vector2 position) => Position = position;
    }

    public struct MouseWheelUp : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseWheelUp(Vector2 position) => Position = position;
    }

    public struct MouseWheelDown : IMouseEvent
    {
        public Vector2 Position { get; }
        public MouseWheelDown(Vector2 position) => Position = position;
    }
}
