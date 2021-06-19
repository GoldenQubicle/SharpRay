using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public struct SnakeConsumedFood : IGameEvent
    {
        public FoodParticle FoodParticle { get; init; }
        public Segment NextSegment { get; init; }
        public int SnakeLength { get; init; }
    }
    public struct SnakeConsumedPoop : IGameEvent { public GameEntity GameEntity { get; init; } }
    public struct SnakeMovement : IGameEvent
    {
        public Direction Direction { get; init; }
        public Vector2 Position { get; init; }
    }
    public struct SnakeCollideWithBody : IGameEvent { }
    public struct SnakeCollideWithBounds : IGameEvent { }
    public struct ParticleSpawn : IGameEvent { }
    public enum Direction { Up, Right, Down, Left }

    public class ParticleSpawner : GameEntity, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        double rndInterval;
        double current;
        private Random Random = new Random();
        double min = 500d;
        double max = 750d;
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
                //rndInterval = double.MaxValue;
                current = 0d;
            }
        }
    }

    public class Segment : GameEntity
    {
        public Vector2 Bounds { get; init; }
        public Direction Direction { get; set; }
        protected Color Color { get; set; }

        private static double interval = 550 * Program.TickMultiplier;
        private double current = 0d;
        private double prevDistance = 0f;
        private Direction previousDirection;

        protected Segment Next { get; set; }
        public override void Render(double deltaTime)
        {

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);

            DoMovement(deltaTime);
            DrawRectangleRounded(Collider, .35f, 1, Color.DARKPURPLE);
            DrawRectangleRoundedLines(Collider, .35f, 1, 2, Color.PURPLE);
        }

        public Segment AddNext()
        {
            Next = new Segment
            {
                Size = Size,
                Direction = Direction,
                Bounds = Bounds,
                Position = Direction switch
                {
                    Direction.Up => Position + new Vector2(0f, 20f),
                    Direction.Right => Position + new Vector2(-20f, 0f),
                    Direction.Down => Position + new Vector2(0f, -20f),
                    Direction.Left => Position + new Vector2(20f, 0f),
                },
            };
            return Next;
        }

        protected bool DoMovement(double deltaTime)
        {
            current += deltaTime;

            var t = Math.Clamp(Program.MapRange(current, 0d, interval, 0d, 1d), 0d, 1d);
            var e = Easings.EaseBackInOut((float)t, 0f, Size.X, 1f);
            var d = e - prevDistance;

            prevDistance = e;
            Position += GetVelocity(d);

            if (current > interval)
            {
                current = 0d;
                prevDistance = 0d;

                return true;
            }
            return false;
        }

        protected Vector2 GetVelocity(double d) => Direction switch
        {
            Direction.Up => new Vector2(0f, -(float)d),
            Direction.Right => new Vector2((float)d, 0f),
            Direction.Down => new Vector2(0f, (float)d),
            Direction.Left => new Vector2(-(float)d, 0f),
        };
        internal void SetDirection(Direction direction)
        {
            previousDirection = Direction;
            Next?.SetDirection(previousDirection);
            Direction = direction;
        }
    }


    public class Head : Segment, IHasCollision, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        private List<Segment> Segments { get; } = new();
        public Direction NextDirection { get; set; }
        private Func<SnakeConsumedFood> OnConsumedFood { get;  set; }
        private bool HasConsumedFood { get;  set; }

        public Head()
        {
            Segments.Add(this);
        }

        public void OnCollision(GameEntity e)
        {
            if (e is FoodParticle f)
            {
                HasConsumedFood = true;
                OnConsumedFood = () =>
                {
                    var next = Segments.Last().AddNext();
                    Segments.Add(next);
                    return new SnakeConsumedFood
                    {
                        FoodParticle = f,
                        NextSegment = next,
                        SnakeLength = Segments.Count
                    };
                };
            }

            if (e is PoopParticle p)
                EmitEvent(new SnakeConsumedPoop { GameEntity = p });

            //if (e is Segment s)
            //    EmitEvent(new SnakeCollideWithBody());

        }

        public override void Render(double deltaTime)
        {

            if (Position.X > Bounds.X) Position = new Vector2(0, Position.Y);
            if (Position.X < 0) Position = new Vector2(Bounds.X, Position.Y);
            if (Position.Y > Bounds.Y) Position = new Vector2(Position.X, 0);
            if (Position.Y < 0) Position = new Vector2(Position.X, Bounds.Y);

            if (DoMovement(deltaTime))
            {
                Next?.SetDirection(Direction);
                Direction = NextDirection;
                EmitEvent(new SnakeMovement { Direction = Direction, Position = Position });

                if (HasConsumedFood)
                {
                    EmitEvent(OnConsumedFood());
                    HasConsumedFood = false;
                }
            }

            DrawRectangleRounded(Collider, .5f, 10, Color.MAGENTA);
            DrawRectangleRoundedLines(Collider, .5f, 10, 1, Color.PURPLE);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed) return;

            NextDirection = e switch
            {
                KeyUp => Direction.Up,
                KeyRight => Direction.Right,
                KeyDown => Direction.Down,
                KeyLeft => Direction.Left,
            };
        }
    }

    public class FoodParticle : GameEntity
    {
        public Color Color { get; init; }
        public override void Render(double deltaTime)
        {
            DrawRectangleRec(Collider, Color.LIME);
            DrawRectangleLinesEx(Collider, 1, Color.GREEN);
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
