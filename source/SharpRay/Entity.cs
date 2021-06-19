using Raylib_cs;
using System;
using System.Numerics;
using System.Transactions;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public interface IMouseListener { void OnMouseEvent(IMouseEvent ke); }
    public interface IKeyBoardListener { void OnKeyBoardEvent(IKeyBoardEvent me); }
    public interface IHasCollision { void OnCollision(GameEntity ge); }
    public interface IHasCollider { public Raylib_cs.Rectangle Collider { get; } }
    public interface IGameEvent : IAudioEvent { }

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


    #region snak gam

    public struct SnakeConsumedFood : IGameEvent { public GameEntity GameEntity { get; init; } }
    public struct SnakeConsumedPoop : IGameEvent { public GameEntity GameEntity { get; init; } }
    public struct SnakeMovement : IGameEvent { }
    public struct SnakeCollideWithBody : IGameEvent { }
    public struct SnakeCollideWithBounds : IGameEvent { }
    public struct ParticleSpawn : IGameEvent { }

    public class ParticleSpawner : GameEntity, IEventEmitter<IGameEvent>
    {
        double rndInterval;
        double current;
        private Random Random = new Random();
        double min = 1250d;
        double max = 1750d;
        public ParticleSpawner()
        {
            rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, min, max) * Program.TickMultiplier;
        }

        public override void Render(double deltaTime)
        {
            current += deltaTime;
            if (current > rndInterval)
            {
                EmitEvent(new ParticleSpawn());
                rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, min, max) * Program.TickMultiplier;
                current = 0d;
            }
        }

        public Action<IGameEvent> EmitEvent { get; set; }
    }

    public class Segment : GameEntity
    {

    }

    public enum Direction { Up, Right, Down, Left }

    public class Head : GameEntity, IHasCollision, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        public Vector2 Bounds { get; init; }

        public void OnCollision(GameEntity e)
        {
            if (e is FoodParticle f)
                EmitEvent(new SnakeConsumedFood { GameEntity = f });

            if (e is PoopParticle p)
                EmitEvent(new SnakeConsumedPoop { GameEntity = p });

            if (e is Segment s)
                EmitEvent(new SnakeCollideWithBody ());

        }

        private Direction direction = Direction.Right;
        private static double interval = 550 * Program.TickMultiplier;
        private static double current = 0d;
        private static double prevDistance = 0f;
        private Func<double, Vector2> Speed = d => new Vector2((float)d, 0f);

        public override void Render(double deltaTime)
        {

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);

            current += deltaTime;

            var t = Math.Clamp(Program.MapRange(current, 0d, interval, 0d, 1d), 0d, 1d);
            var e = Easings.EaseBackInOut((float)t, 0f, Size.X, 1f);
            var d = e - prevDistance;

            prevDistance = e;
            Position += Speed(d);

            if (current > interval)
            {
                current = 0d;
                prevDistance = 0d;
                Speed = d => direction switch
                {
                    Direction.Up => new Vector2(0f, -(float)d),
                    Direction.Right => new Vector2((float)d, 0f),
                    Direction.Down => new Vector2(0f, (float)d),
                    Direction.Left => new Vector2(-(float)d, 0f),
                };

                EmitEvent(new SnakeMovement ());
            }

            DrawRectangleV(Position, Size, Color.MAGENTA);

        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyUp) direction = Direction.Up;
            if (e is KeyRight) direction = Direction.Right;
            if (e is KeyDown) direction = Direction.Down;
            if (e is KeyLeft) direction = Direction.Left;
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

    public class PoopParticle : GameEntity
    {
        public Color Color { get; init; }
        public override void Render(double deltaTime)
        {
            DrawRectangleRec(Collider, Color);
        }
    }

    #endregion
}
