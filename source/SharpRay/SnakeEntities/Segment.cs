﻿using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.SnakeConfig;

namespace SharpRay
{
    public enum Direction { Up, Right, Down, Left }

    public class Segment : GameEntity
    {
        public Vector2 Bounds { get; init; }
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }

        protected bool IntervalElapsed { get; set; }
        protected Vector2 Center { get; set; }
        protected Segment Next { get; set; }
        protected Color Color { get; set; }

        private static double interval = LocomotionInterval * Program.TickMultiplier;
        private double current = 0d;
        private double prevDistance = 0f;
        private Direction previousDirection;

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
                prevDistance = 0d;
                IntervalElapsed = true;
            }

            DoLocomotion();
            
            //update position used by collider
            Position = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2);
        }

        public override void Render()
        {
            DrawRectangleRounded(Collider, .35f, 1, Color.DARKPURPLE);
            DrawRectangleRoundedLines(Collider, .35f, 1, 2, Color.PURPLE);

            DrawCircleV(Center, 2, Color.BLUE);
        }

        public Segment SetNext()
        {
            Next = new Segment
            {
                Size = new Vector2(SegmentSize, SegmentSize),
                Direction = Direction,
                Center = Direction switch
                {
                    Direction.Up => Center + new Vector2(0f, CellSize),
                    Direction.Right => Center + new Vector2(-CellSize, 0f),
                    Direction.Down => Center + new Vector2(0f, -CellSize),
                    Direction.Left => Center + new Vector2(CellSize, 0f),
                },
                Position = Center - Size / 2
            };
            return Next;
        }

        public void SetDirection(Direction direction)
        {
            previousDirection = Direction;
            Next?.SetDirection(previousDirection);
            Direction = direction;
        }

        private void DoLocomotion()
        {
            var t = Math.Clamp(Program.MapRange(current, 0d, interval, 0d, 1d), 0d, 1d);
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
