using Raylib_cs;
using static Raylib_cs.Raylib;
using static ShittySnake.Settings;
using static SharpRay.Core.SharpRayConfig;
using static SharpRay.Core.Application;
using SharpRay.Entities;
using SharpRay.Collision;
using SnakeEvents;

namespace SnakeEntities
{
    public class ParticlePoop : GameEntity
    {
        private float alpha;
        private double current;
        private double interval = PoopDespawnInterval * TickMultiplier;
        private RectCollider Collider { get; set; }

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
                EmitEvent(new DespawnPoop { PoopParticle = this });
            }

            var t = MapRange(current, 0d, interval, 1d, 0d);
            alpha = Easings.EaseBounceInOut((float)t, 0f, 37f, 1f);
        }

        public override void Render()
        {
            DrawRectangleRounded(Collider.Rect, .5f, 5, ColorAlpha(Color.DARKBROWN, alpha));
            DrawRectangleRoundedLines(Collider.Rect, .5f, 5, 2, Color.BROWN);
        }
    }
}
