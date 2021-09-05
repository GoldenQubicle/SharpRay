using Raylib_cs;
using SharpRay.Entities;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace ProtoCity
{
    public class Agent : GameEntity
    {
        private Vector2 Velocity = Vector2.UnitX;
        private float Speed;
        private float MaxSpeed  = 1f;
        private float TurnFactor = .5f;
        private int Margin = 250;

        public override void Render()
        {
            DrawCircleV(Position, 3, Color.PINK);
            DrawLineV(Position, Position + (Velocity * 20), Color.RED);

            DrawRectangleLines(Margin, Margin, Program.WindowWidth - Margin*2, Program.WindowHeight - Margin*2, Color.DARKGRAY);

        }
        public override void Update(double deltaTime)
        {
            if (Position.X < Margin)
                Velocity -= new Vector2(0f, TurnFactor);
            if (Position.X > Program.WindowWidth - Margin)
                Velocity += new Vector2(0f, TurnFactor);
            if (Position.Y < Margin)
                Velocity += new Vector2(TurnFactor, 0f);
            if (Position.Y > Program.WindowHeight - Margin)
                Velocity -= new Vector2(TurnFactor, 0f);
            
            Speed = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (Speed > MaxSpeed)
                Velocity = Velocity / Speed * MaxSpeed;

            Position += Velocity;
        }
    }
}
