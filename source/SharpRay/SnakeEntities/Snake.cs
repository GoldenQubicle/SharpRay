using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.SnakeConfig;

namespace SharpRay
{
    public class Snake : Segment, IHasCollision
    {

        public List<Segment> Segments { get; } = new();
        private Func<SnakeConsumedFood> OnConsumedFood { get; set; }
        private Func<SnakeConsumedPoop> OnConsumedPoop { get; set; }

        public Snake(Vector2 position)
        {
            Position = position;
            Size = new Vector2(HeadSize, HeadSize);
            Segments.Add(this);
            Center = new Vector2(Position.X + CellSize / 2, Position.Y + CellSize / 2);
        }

        public void OnCollision(GameEntity e)
        {
            if (e is ParticleFood f)
                OnConsumedFood = () =>
                {
                    var next = Segments.Last().SetNext();
                    Segments.Add(next);
                    return new SnakeConsumedFood
                    {
                        FoodParticle = f,
                        NextSegment = next,
                        SnakeLength = Segments.Count
                    };
                };

            if (e is ParticlePoop p)
                OnConsumedPoop = () =>
                {
                    var tail = Segments.Last();
                    Segments.Remove(tail);
                    return new SnakeConsumedPoop
                    {
                        PoopParticle = p,
                        Tail = tail
                    };
                };

            if (e is Segment s && Segments[1] != s) //ignore first segment collision due to locomotion
                EmitEvent(new SnakeGameOver { Score = Segments.Count });
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (Center.X > Bounds.X) Center = new Vector2(0, Center.Y);
            if (Center.X < 0) Center = new Vector2(Bounds.X, Center.Y);
            if (Center.Y > Bounds.Y) Center = new Vector2(Center.X, 0);
            if (Center.Y < 0) Center = new Vector2(Center.X, Bounds.Y);

            if (IntervalElapsed)
            {
                Next?.SetDirection(Direction);
                Direction = NextDirection;

                EmitEvent(new SnakeLocomotion { Direction = Direction, Position = Position });

                if (OnConsumedFood is not null)
                {
                    EmitEvent(OnConsumedFood());
                    OnConsumedFood = null;
                    Next.SetIsDigesting(true);
                }

                if (OnConsumedPoop is not null)
                {
                    EmitEvent(OnConsumedPoop());
                    OnConsumedPoop = null;
                }

                IntervalElapsed = false;
            }

            if (Center.X >= Bounds.X || Center.X <= 0 || Center.Y >= Bounds.Y || Center.Y <= 0)
                EmitEvent(new SnakeGameOver { Score = Segments.Count });
        }

        public override void Render()
        {
            DrawRectangleRounded(Collider, .5f, 10, Color.MAGENTA);
            DrawRectangleRoundedLines(Collider, .5f, 10, 1, Color.PURPLE);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            NextDirection = e switch
            {
                SnakeUp => Direction.Up,
                SnakeRight => Direction.Right,
                SnakeDown => Direction.Down,
                SnakeLeft => Direction.Left,
            };
        }
    }
}
