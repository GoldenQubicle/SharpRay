using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Eventing;
using SnakeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Raylib_cs.Raylib;
using static ShittySnake.Settings;

namespace SnakeEntities
{
    public class Snake : Segment, IHasCollision
    {
        public List<Segment> Segments { get; } = new();
        private Func<SnakeConsumedFood> OnConsumedFood { get; set; }
        private Func<DespawnPoop> OnConsumedPoop { get; set; }
        private RectCollider Collider { get; set; }

        public Snake(Vector2 position)
        {
            Position = position;
            Size = new Vector2(HeadSize, HeadSize);
            Segments.Add(this);
            Center = new Vector2(Position.X + CellSize / 2, Position.Y + CellSize / 2);
            Collider = new RectCollider
            {
                Position = Position,
                Size = Size,
            };
        }

        public void OnCollision(IHasCollider e)
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

            //ignore first segment collision due to locomotion
            if ((e is Segment s && Segments[1] != s) || e is ParticlePoop p)
                EmitEvent(new SnakeGameOver { Score = Segments.Count });

        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

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
            DrawRectangleRounded(Collider.Rect, .5f, 10, Color.MAGENTA);
            DrawRectangleRoundedLines(Collider.Rect, .5f, 10, 1, Color.PURPLE);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            NextDirection = e switch
            {
                SnakeUp => Direction.Up,
                SnakeRight => Direction.Right,
                SnakeDown => Direction.Down,
                SnakeLeft => Direction.Left,
                _ => throw new NotImplementedException(),
            };
        }

        
    }
}
