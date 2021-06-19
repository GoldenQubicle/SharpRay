using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public enum Direction { Up, Right, Down, Left }

    public class Segment : GameEntity
    {
        public Vector2 Bounds { get; init; }
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }

        protected Segment Next;
        protected Color Color;

        private static double interval = 550 * Program.TickMultiplier;
        private double current = 0d;
        private double prevDistance = 0f;
        private Direction previousDirection;

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

        public void SetDirection(Direction direction)
        {
            previousDirection = Direction;
            Next?.SetDirection(previousDirection);
            Direction = direction;
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

        private Vector2 GetVelocity(double d) => Direction switch
        {
            Direction.Up => new Vector2(0f, -(float)d),
            Direction.Right => new Vector2((float)d, 0f),
            Direction.Down => new Vector2(0f, (float)d),
            Direction.Left => new Vector2(-(float)d, 0f),
        };
    }
}
