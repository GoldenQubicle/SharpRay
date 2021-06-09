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
    public interface ICollisionListener { void OnCollisionEvent(ICollisionEvent e); }

    public abstract class Entity : IKeyBoardListener, IMouseListener, IDrawable
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; init; }
        public Raylib_cs.Rectangle Rectangle
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }

        public virtual void Draw() { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }

    }


    public class Player : Entity, ICollisionListener
    {
        public Vector2 Bounds { get; init; }

        private float Speed = .075f;

        public void OnCollisionEvent(ICollisionEvent e)
        {
            if (e is FoodParticle f)
            {

            }

            if (e is PoisonParticle p)
            {

            }
        }
        public override void Draw()
        {
            DrawRectangleV(Position, Size, Color.PURPLE);

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

    public class FoodParticle : Entity
    {
        public Color Color { get; init; }
        public override void Draw()
        {
            DrawRectangleRec(Rectangle, Color);
        }
    }

    public class PoisonParticle : Entity
    {
        public Color Color { get; init; }
        public override void Draw()
        {
            DrawRectangleRec(Rectangle, Color);
        }
    }
}
