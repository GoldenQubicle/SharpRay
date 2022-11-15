namespace SnakeEntities
{
    public class ParticleFood : Entity, IHasUpdate, IHasRender, IHasCollider
    {
        private float prevDistance;
        private double current;
        private double interval = FoodSwayInterval * TickMultiplier;

        public ParticleFood(Vector2 position, int foodSize)
        {
            Position = position;
            Size = new Vector2(foodSize, foodSize);
            Collider = new RectCollider
            {
                Position = position,
                Size = Size,
            };
        }

        public ICollider Collider { get; set; }
        public int FoodSize { get; }

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
                current = 0d;

            var t = MapRange(current, 0d, interval, 0d, Math.Tau);
            var e = MathF.Sin((float)t) * 5;
            var d = e - prevDistance;
            prevDistance = e;
            Position += new Vector2(0f, (float)d);
            (Collider as RectCollider).Position = Position;
        }

        public override void Render()
        {
            DrawRectangleRounded((Collider as RectCollider).Rect, .5f, 1, Color.LIME);
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .5f, 2, 1, Color.GREEN);
        }
    }
}
