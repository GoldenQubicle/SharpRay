using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public interface IMouseListener { void OnMouseEvent(IMouseEvent ke); }
    public interface IKeyBoardListener { void OnKeyBoardEvent(IKeyBoardEvent me); }
    public interface IHasCollision { void OnCollision(GameEntity ge); }
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

    public interface IGameEvent : IAudioEvent { }
    public struct PlayerConsumedParticle : IGameEvent { public GameEntity GameEntity { get; init; } }
    public struct PlayerMovement : IGameEvent { public GameEntity GameEntity { get; init; } }

    public class Player : GameEntity, IHasCollision, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        public Vector2 Bounds { get; init; }


        public void OnCollision(GameEntity e)
        {
            if (e is FoodParticle f)
                EmitEvent(new PlayerConsumedParticle { GameEntity = f });

            if (e is PoisonParticle p)
                EmitEvent(new PlayerConsumedParticle { GameEntity = p });

        }

        static double interval = 750 * Program.TickMultiplier;
        static double current = 0d;
        static double prevDistance = 0f;

        public override void Render(double deltaTime)
        {
            DrawRectangleV(Position, Size, Color.PURPLE);

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);

            current += deltaTime;
            if (current > interval)
            {
                EmitEvent(new PlayerMovement { GameEntity = this });
                current = 0d;
                prevDistance = 0d;
            }
            
            var e = Easings.EaseSineInOut((float)current, 0f, Size.X, (float)interval);
            var d = e - prevDistance;
            prevDistance = e;
            Position += new Vector2((float)d, 0f);

        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            float Speed = 2.5f;

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
