using SharpRay.Components;

namespace SnakeEntities
{
    public class ParticlePoop : Entity, IHasUpdate, IHasRender, IHasCollider, IEventEmitter<IGameEvent>
    {
        private float alpha;
        private double current;
        private readonly double interval = PoopDespawnInterval * TickMultiplier;
        public ICollider Collider { get; set; }
        public Action<IGameEvent> EmitEvent { get; set; }

        public ParticlePoop(Vector2 position, int poopSize)
        {
            Position = position;
            Size = new Vector2(poopSize);
            Collider = new RectCollider
            {
                Position = position,
                Size = Size,
            };
        }


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
            DrawRectangleRounded((Collider as RectCollider).Rect, .5f, 5, ColorAlpha(Color.DARKBROWN, alpha));
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .5f, 5, 2, Color.BROWN);
        }
    }
}
