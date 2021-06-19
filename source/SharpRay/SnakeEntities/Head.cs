using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class Head : Segment, IHasCollision, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }
        private List<Segment> Segments { get; } = new();
        private Func<SnakeConsumedFood> OnConsumedFood { get; set; }
        private bool HasConsumedFood { get; set; }
        public Head() => Segments.Add(this);

        public void OnCollision(GameEntity e)
        {
            if (e is ParticleFood f)
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

            if (e is ParticlePoop p)
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
}
