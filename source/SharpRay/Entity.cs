using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public interface IDrawable { void Draw(); }
    public interface ILoop { void Update(Vector2 mPos); }
    public interface IMouseListener { void OnMouseEvent(IMouseEvent e); }
    public interface IKeyBoardListener { void OnKeyBoardEvent(IKeyBoardEvent e); }
    public interface IEventEmitter<TEvent> where TEvent : IEvent
    {
        Action<TEvent> EmitEvent { get; set; }
    }

    public abstract class Entity : IKeyBoardListener, IMouseListener, IDrawable
    {
        public virtual void Draw() { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }
    }

    public class Player : Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; init; }
        public Vector2 Bounds { get; init; }

        private float Speed = .075f;

        public override void Draw()
        {
            DrawRectangleV(Position, Size, Color.ORANGE);

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyUp) Position -= new Vector2(0f, Speed);
            if (e is KeyRight) Position += new Vector2(Speed, 0f);
            if (e is KeyDown) Position += new Vector2(0f, Speed);
            if (e is KeyLeft) Position -= new Vector2(Speed, 0f);
        }
    }
}
