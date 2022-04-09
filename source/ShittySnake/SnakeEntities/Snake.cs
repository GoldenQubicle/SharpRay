using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Interfaces;
using SnakeEvents;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static ShittySnake.Settings;

namespace SnakeEntities
{
    public class Snake : Segment, IHasCollision
    {
        private Func<SnakeConsumedFood> OnConsumedFood { get; set; }
        private Func<DespawnPoop> OnConsumedPoop { get; set; }
        private int _segmentCount;
        public Snake(Vector2 position)
        {
            Position = position;
            Size = new Vector2(HeadSize, HeadSize);
            Center = new Vector2(Position.X + CellSize / 2, Position.Y + CellSize / 2);
            Collider = new RectCollider
            {
                Position = position,
                Size = Size,
            };
            _segmentCount = 1;
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is ParticleFood f)
            {
                _segmentCount++;
                SetIsDigesting(true);
                Console.WriteLine("Hello");

                var last = Next;
                while (last.Next is not null)
                    last = last.Next;

                OnConsumedFood = () =>
                {
                    return new SnakeConsumedFood
                    {
                        FoodParticle = f,
                        NextSegment = last.SetNext(),
                        SnakeLength = _segmentCount
                    };
                };
            }

            //ignore first segment collision due to locomotion
            if (e is Segment s && s != Next || e is ParticlePoop p)
                EmitEvent(new SnakeGameOver { Score = _segmentCount });

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
                EmitEvent(new SnakeGameOver { Score = _segmentCount });
        }

        public override void Render()
        {
            DrawRectangleRounded((Collider as RectCollider).Rect, .5f, 10, Color.MAGENTA);
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .5f, 10, 1, Color.PURPLE);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e) =>
            NextDirection = e switch
            {
                KeyUpDown => Direction.Up,
                KeyRightDown => Direction.Right,
                KeyDownDown => Direction.Down,
                KeyLeftDown => Direction.Left,
                _ => NextDirection
            };
    }
}
