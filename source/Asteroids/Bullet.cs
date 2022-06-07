namespace Asteroids
{
    public class Bullet : Entity, IHasCollider, IHasRender, IHasUpdate
    {
        public record struct Data(
        Vector2 Origin,
        float Angle,
        float InitialForce,
        float Radius,
        float Speed,
        double LifeTime,
        int Damage,
        Action<Vector2, float> Render);

        public enum Type
        {
            Simple,
            Medium,
            Heavy
        }

        public static Data GetData(Vector2 origin, float angle, float initialForce, Type type) => type switch
        {
            Type.Simple => new(Origin: origin, Angle: angle, InitialForce: initialForce,
                Radius: 2f, Speed: 10f, LifeTime: 750d, Damage: 1, Render: (p, r) =>
                {
                    DrawCircleV(p, r, Color.YELLOW);
                }),
            Type.Medium => new(Origin: origin, Angle: angle, InitialForce: initialForce,
                Radius: 3f, Speed: 10f, LifeTime: 750d, Damage: 2, Render: (p, r) =>
                {
                    DrawCircleV(p, r, Color.GOLD);
                }),
            Type.Heavy => new(Origin: origin, Angle: angle, InitialForce: initialForce,
                Radius: 4f, Speed: 10f, LifeTime: 750d, Damage: 3, Render: (p, r) =>
                {
                    DrawCircleV(p, r, Color.ORANGE);
                    DrawCircleV(p, r-1, Color.GOLD);
                    DrawCircleV(p, r-2, Color.RAYWHITE);
                }),
            _ => throw new NotImplementedException(),
        };

        public ICollider Collider { get; }
        public int Damage { get; private set; }

        private Vector2 acceleration;
        private readonly float radius;
        private readonly double lifeTime;
        private double elapsed;

        private readonly Action<Vector2, float> RenderAction;
        public Bullet(Data data)
        {
            Damage = data.Damage;
            RenderAction = data.Render;
            Position = data.Origin;
            lifeTime = data.LifeTime * SharpRayConfig.TickMultiplier;
            radius = data.Radius;
            acceleration = new Vector2(MathF.Cos(data.Angle) * (data.Speed + data.InitialForce), MathF.Sin(data.Angle) * (data.Speed + data.InitialForce));
            
            Collider = new CircleCollider
            {
                Center = Position,
                Radius = data.Radius
            };

            RenderLayer = RlAsteroidsBullets;
        }

        public override void Render()
        {
            RenderAction(Position, radius);
            //Collider.Render();
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused) return;

            elapsed += deltaTime;

            if (elapsed > lifeTime)
                RemoveEntity(this);

            Position += acceleration;

            (Collider as CircleCollider).Center = Position;

            //bounds check
            if (Position.X < 0) Position = new Vector2(WindowWidth, Position.Y);
            if (Position.X > WindowWidth) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, WindowHeight);
            if (Position.Y > WindowHeight) Position = new Vector2(Position.X, 0);
        }
    }
}
