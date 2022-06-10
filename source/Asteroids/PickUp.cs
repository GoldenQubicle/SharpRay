﻿namespace Asteroids
{
    public class PickUp : Entity, IHasCollider, IHasRender, IHasUpdate
    {
        public enum Type
        {
            Bullet,
            Weapon
        }

        public const string PickupSound = nameof(PickupSound);
        public const string SpawnSound = nameof(SpawnSound);
        public ICollider Collider { get; }
        public Action OnPickUp { get; init; }
        public string Description { get; init; }
        public int SpawnScore { get; init; }
        public bool HasSpawned { get; set; }
        public Type PickupType { get; init; }

        private Font Font = GetFont(FontFutureThin);
        private (string t, Color fill, Color outline, Color text, Vector2 offset) Data;
        private float prevDistance;
        private double current;
        private double interval = 2000d * SharpRayConfig.TickMultiplier;

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
            (Collider as RectCollider).Position = pos;
            Data = GetData();
            HasSpawned = true;
            AddEntity(this);
            PlaySound(SpawnSound);
        }

        public override void Render()
        {
            var r = (Collider as RectCollider).Rect;
            
            DrawRectangleRounded(r, .5f, 8, Data.fill);
            DrawRectangleRoundedLines(r, .5f, 8, 3, Data.outline);
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
            (Collider as RectCollider).Position = Position;
        }

        private (string t, Color fill, Color outline, Color text, Vector2 offset) GetData() => PickupType switch
        {
            Type.Bullet => ("B", Color.YELLOW, Color.GOLD, Color.ORANGE, new Vector2(5, -2)),
            Type.Weapon => ("W", Color.PURPLE, Color.PINK, Color.DARKPURPLE, new Vector2(4, -2)),
        };
    }
}
