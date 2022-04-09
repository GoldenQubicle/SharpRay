using Raylib_cs;
using SharpRay.Entities;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static ShittySnake.Settings;
using static SharpRay.Core.SharpRayConfig;
using static SharpRay.Core.Application;
using SharpRay.Collision;
using SnakeEvents;
using SharpRay.Interfaces;

namespace SnakeEntities
{
    public enum Direction { Up, Right, Down, Left }

    public class Segment : GameEntity, IHasCollider
    {
        public Vector2 Bounds { get; init; }
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }
        protected bool IntervalElapsed { get; set; }
        protected Vector2 Center { get; set; }
        public Segment Next { get; set; }
        protected Color Color { get; set; }
        public ICollider Collider { get; set; }

        private static double interval = LocomotionInterval * TickMultiplier;
        private double current = 0d;
        private double prevDistance = 0f;
        private bool isDigesting = false;

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
                prevDistance = 0d;
                IntervalElapsed = true;

                if (isDigesting && Next is not null)
                {
                    Next.SetIsDigesting(true);
                    isDigesting = false;
                }

                if (isDigesting && Next is null)
                {
                    EmitEvent(new PoopParticleSpawn
                    {
                        Position = Center - new Vector2(CellSize - PoopSize, CellSize - PoopSize) / 2
                    });
                    isDigesting = false;
                }
            }

            DoLocomotion();

            //update position used by collider
            (Collider as RectCollider).Position = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2);
        }

        public override void Render()
        {
            Color = isDigesting ? Color.BROWN : Color.DARKPURPLE;

            DrawRectangleRounded((Collider as RectCollider).Rect, .35f, 1, Color);
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .35f, 1, 2, Color.PURPLE);

            DrawCircleV(Center, 4, Color.YELLOW);
        }

        public Segment SetNext()
        {
            Next = new Segment
            {
                Position = Center - Size / 2,
                RenderLayer = RenderLayer,
                Size = new Vector2(SegmentSize, SegmentSize),
                Direction = Direction,
                Bounds = Bounds,
                Center = Direction switch
                {
                    Direction.Up => Center + new Vector2(0f, CellSize),
                    Direction.Right => Center + new Vector2(-CellSize, 0f),
                    Direction.Down => Center + new Vector2(0f, -CellSize),
                    Direction.Left => Center + new Vector2(CellSize, 0f),
                },
                Collider = new RectCollider
                {
                    Position = Position,
                    Size = new Vector2(SegmentSize, SegmentSize)
                }
            };
            return Next;
        }

        public void SetDirection(Direction nextDirection)
        {
            Next?.SetDirection(Direction);
            Direction = nextDirection;
        }

        public void SetIsDigesting(bool b) => isDigesting = b;

        private void DoLocomotion()
        {
            var t = Math.Clamp(MapRange(current, 0d, interval, 0d, 1d), 0d, 1d);
            var e = Easings.EaseBackInOut((float)t, 0f, CellSize, 1f);
            var d = e - prevDistance;
            prevDistance = e;

            Center += Direction switch
            {
                Direction.Up => new Vector2(0f, -(float)d),
                Direction.Right => new Vector2((float)d, 0f),
                Direction.Down => new Vector2(0f, (float)d),
                Direction.Left => new Vector2(-(float)d, 0f),
            };
        }
    }
}
