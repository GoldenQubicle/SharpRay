using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticleFood : GameEntity
    {
        private float prevDistance;
        private double current;
        private double interval = 750 * Program.TickMultiplier;

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
            }

            var t = Program.MapRange(current, 0d, interval, 0d, Math.Tau);
            var e = MathF.Sin((float)t) * 5;
            var d = e - prevDistance;
            prevDistance = e;
            Position += new Vector2(0f, (float)d);
        }

        public override void Render()
        {
            DrawRectangleRounded(Collider, .5f, 1, Color.LIME);
            DrawRectangleRoundedLines(Collider, .5f, 2, 1, Color.GREEN);
        }
    }
}
