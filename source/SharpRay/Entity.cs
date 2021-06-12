using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public interface IMouseListener { void OnMouseEvent(IMouseEvent e); }
    public interface IKeyBoardListener { void OnKeyBoardEvent(IKeyBoardEvent e); }
    public interface IHasCollision { void OnCollision(GameEntity e); }
    public interface IHasCollider { public Raylib_cs.Rectangle Collider { get; } }

    public abstract class Entity : IKeyBoardListener, IMouseListener
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; init; }

        /// <summary>
        /// Delta time is the interval since last render frame in ticks!
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Render(double deltaTime) { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }

    }

    public abstract class GameEntity : Entity, IHasCollider
    {
        public Raylib_cs.Rectangle Collider
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
    }

    public interface IPlayerEvent : IEvent { }
    public struct PlayerConsumedParticle : IPlayerEvent { public GameEntity GameEntity { get; init; } }

    public class Player : GameEntity, IHasCollision, IEventEmitter<IPlayerEvent>
    {
        public Action<IPlayerEvent> EmitEvent { get; set; }

        public Vector2 Bounds { get; init; }

        private float Speed = .075f;

        public void OnCollision(GameEntity e)
        {
            if (e is FoodParticle f)
                EmitEvent(new PlayerConsumedParticle { GameEntity = f });

            if (e is PoisonParticle p)
                EmitEvent(new PlayerConsumedParticle { GameEntity = p });

        }

        double elapsed = 0d;
        //specified in milliseconds * tick multiplier
        //reason being when runnning uncapped fps delta time in millis is often zero so we need higher precision
        double interval = 500d * Program.TickMultiplier;
        public override void Render(double deltaTime)
        {
            DrawRectangleV(Position, Size, Color.PURPLE);

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);

            elapsed += deltaTime;
            if (elapsed > interval)
                elapsed = 0d;

            var phi = (float)Program.MapRange(elapsed, 0d, interval, 0d, Math.Tau);

            var x = MathF.Cos(phi) * 100;
            var y = MathF.Sin(phi) * 100;

            Position = new Vector2(x + 250, y + 100);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyUp) Position -= new Vector2(0f, Speed);
            if (e is KeyRight) Position += new Vector2(Speed, 0f);
            if (e is KeyDown) Position += new Vector2(0f, Speed);
            if (e is KeyLeft) Position -= new Vector2(Speed, 0f);
        }
    }

    public class FoodParticle : GameEntity
    {
        public Color Color { get; init; }
        public override void Render(double deltaTime)
        {
            DrawRectangleRec(Collider, Color);
        }
    }

    public class PoisonParticle : GameEntity
    {
        public Color Color { get; init; }
        public override void Render(double deltaTime)
        {
            DrawRectangleRec(Collider, Color);
        }
    }
}
