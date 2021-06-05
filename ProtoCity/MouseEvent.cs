using System;
using System.Numerics;

namespace ProtoCity
{
    public interface IMouseEvent
    {
        public Vector2 Position { get; }
    }

    public struct MouseLeftClick : IMouseEvent, IEquatable<MouseLeftClick>
    {
        public Vector2 Position { get; }
        public MouseLeftClick(Vector2 position) => Position = position;
        public bool Equals(MouseLeftClick other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseLeftClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseLeftClick left, MouseLeftClick right) => left.Equals(right);
        public static bool operator !=(MouseLeftClick left, MouseLeftClick right) => !left.Equals(right);
    }

    public struct MouseMiddleClick : IMouseEvent, IEquatable<MouseMiddleClick>
    {
        public Vector2 Position { get; }
        public MouseMiddleClick(Vector2 position) => Position = position;
        public bool Equals(MouseMiddleClick other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseMiddleClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseMiddleClick left, MouseMiddleClick right) => left.Equals(right);
        public static bool operator !=(MouseMiddleClick left, MouseMiddleClick right) => !left.Equals(right);
    }

    public struct MouseRighttClick : IMouseEvent, IEquatable<MouseRighttClick>
    {
        public Vector2 Position { get; }
        public MouseRighttClick(Vector2 position) => Position = position;
        public bool Equals(MouseRighttClick other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseRighttClick click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseRighttClick left, MouseRighttClick right) => left.Equals(right);
        public static bool operator !=(MouseRighttClick left, MouseRighttClick right) => !left.Equals(right);
    }

    public struct MouseLeftDrag: IMouseEvent, IEquatable<MouseLeftDrag>
    {
        public Vector2 Position { get; }
        public MouseLeftDrag(Vector2 position) => Position = position;
        public bool Equals(MouseLeftDrag other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseLeftDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseLeftDrag left, MouseLeftDrag right) => left.Equals(right);
        public static bool operator !=(MouseLeftDrag left, MouseLeftDrag right) => !left.Equals(right);
    }

    public struct MouseMiddleDrag : IMouseEvent, IEquatable<MouseMiddleDrag>
    {
        public Vector2 Position { get; }
        public MouseMiddleDrag(Vector2 position) => Position = position;
        public bool Equals(MouseMiddleDrag other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseMiddleDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseMiddleDrag left, MouseMiddleDrag right) => left.Equals(right);
        public static bool operator !=(MouseMiddleDrag left, MouseMiddleDrag right) => !left.Equals(right);
    }

    public struct MouseRightDrag : IMouseEvent, IEquatable<MouseRightDrag>
    {
        public Vector2 Position { get; }
        public MouseRightDrag(Vector2 position) => Position = position;
        public bool Equals(MouseRightDrag other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseRightDrag click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseRightDrag left, MouseRightDrag right) => left.Equals(right);
        public static bool operator !=(MouseRightDrag left, MouseRightDrag right) => !left.Equals(right);
    }

    public struct MouseWheelUp : IMouseEvent, IEquatable<MouseWheelUp>
    {
        public Vector2 Position { get; }
        public MouseWheelUp(Vector2 position) => Position = position;
        public bool Equals(MouseWheelUp other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseWheelUp click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseWheelUp left, MouseWheelUp right) => left.Equals(right);
        public static bool operator !=(MouseWheelUp left, MouseWheelUp right) => !left.Equals(right);
    }

    public struct MouseWheelDown : IMouseEvent, IEquatable<MouseWheelDown>
    {
        public Vector2 Position { get; }
        public MouseWheelDown(Vector2 position) => Position = position;
        public bool Equals(MouseWheelDown other) => Equals(other);
        public override bool Equals(object obj) => obj is MouseWheelDown click && Position.Equals(click.Position);
        public override int GetHashCode() => HashCode.Combine(Position);
        public static bool operator ==(MouseWheelDown left, MouseWheelDown right) => left.Equals(right);
        public static bool operator !=(MouseWheelDown left, MouseWheelDown right) => !left.Equals(right);
    }
}
