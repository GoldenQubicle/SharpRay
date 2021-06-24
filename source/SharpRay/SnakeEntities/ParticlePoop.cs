using Raylib_cs;
using static Raylib_cs.Raylib;
using static SharpRay.SnakeConfig;

namespace SharpRay
{
    public class ParticlePoop : GameEntity
    {
        private float alpha;
        private double current;
        private double interval = PoopDespawnInterval * Program.TickMultiplier;

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
                EmitEvent(new DespawnPoop { PoopParticle = this });
            }

            var t = Program.MapRange(current, 0d, interval, 1d, 0d);
            alpha = Easings.EaseBounceInOut((float)t, 0f, 20f, 1f);
        }

        public override void Render()
        {
            DrawRectangleRounded(Collider, .5f, 5, ColorAlpha(Color.DARKBROWN, alpha));
            DrawRectangleRoundedLines(Collider, .5f, 5, 2, Color.BROWN);
        }
    }
}
