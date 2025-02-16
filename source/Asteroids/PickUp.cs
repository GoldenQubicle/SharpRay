namespace Asteroids
{
    public class PickUp : Entity, IHasCollider, IHasRender, IHasUpdate
    {
        public enum Type
        {
            Bullet,
            Weapon,
            Health
        }

        public const string PickupSound = nameof(PickupSound);
        public const string SpawnSound = nameof(SpawnSound);
        public ICollider Collider { get; }
        public Action OnPickUp { get; init; }
        public string Description { get; init; }
        public int SpawnScore { get; init; }
        public Type PickupType { get; init; }
        public bool CanSpawn { get; private set; }

        private Font Font = GetFont(FontFutureThin);
        private (string t, Color fill, Color outline, Color text, Vector2 offset) Data;
        private float prevDistance;
        private double current;
        private double interval = 2000d * SharpRayConfig.TickMultiplier;
        private int trigger;
        public PickUp()
        {
            Size = new Vector2(25, 25);
            Collider = new RectCollider
            {
                Size = Size
            };

            RenderLayer = RlPickUps;
        }

        public void OnSpawn(Vector2 pos)
        {
            Position = pos;
            Collider.Position = pos;
            Data = GetData(PickupType);
            AddEntity(this);
            PlaySound(SpawnSound);
            CanSpawn = false;
        }

        public void UpdateScore(int score)
        {
            if (trigger >= SpawnScore) return;

            trigger += score;

            if (trigger >= SpawnScore)
            {
                CanSpawn = true;
            }
        }

        public void Reset(float resetRatio)
        {
            if (trigger >= SpawnScore)
            {
                trigger = (int)(SpawnScore * resetRatio);
            }
        }

        public override void Render()
        {
            var r = (Collider as RectCollider).Rect;

            DrawRectangleRounded(r, .5f, 8, Data.fill);
            DrawRectangleRoundedLines(r, .5f, 8, Data.outline);
            DrawTextEx(Font, Data.t, Position + Data.offset, 32, 0, Data.text);

            //Collider.Render();
        }

        //TODO simple sway copies from Snake, wrap it as SimpleSine Easing
        // or just replace with Easing...
        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
                current = 0d;

            var t = MapRange(current, 0d, interval, 0d, Math.Tau);
            var e = MathF.Sin((float)t) * 7;
            var d = e - prevDistance;
            prevDistance = e;
            Position += new Vector2(0f, (float)d);
            Collider.Position = Position;
        }

        private (string t, Color fill, Color outline, Color text, Vector2 offset) GetData(Type pickupType) => pickupType switch
        {
            Type.Bullet => ("B", Color.Yellow, Color.Gold, Color.Orange, new Vector2(5, -2)),
            Type.Weapon => ("W", Color.Purple, Color.Pink, Color.DarkPurple, new Vector2(4, -2)),
            Type.Health => ("H", Color.DarkGreen, Color.Lime, Color.Green, new Vector2(5.25f, -2)),
        };
    }
}
