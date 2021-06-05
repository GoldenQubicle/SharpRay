using Raylib_cs;
using System;
using System.Numerics;

namespace ProtoCity
{
    public interface MouseEvent
    {
        public Vector2 Position { get; }
    }

    public struct MouseLeftClick : MouseEvent
    {
        public Vector2 Position { get; }
        public MouseLeftClick(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseLeftClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }

    public struct MouseMiddleClick : MouseEvent
    {
        public Vector2 Position { get; }
        public MouseMiddleClick(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseMiddleClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }

    public struct MouseRighttClick : MouseEvent
    {
        public Vector2 Position { get; }
        public MouseRighttClick(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseRighttClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }

    public struct MouseLeftDrag: MouseEvent
    {
        public Vector2 Position { get; }
        public MouseLeftDrag(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseLeftDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }

    public struct MouseMiddleDrag : MouseEvent
    {
        public Vector2 Position { get; }
        public MouseMiddleDrag(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseMiddleDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }

    public struct MouseRightDrag : MouseEvent
    {
        public Vector2 Position { get; }
        public MouseRightDrag(Vector2 position) => Position = position;
        public override bool Equals(object obj) => obj is MouseRightDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
    }
}
