namespace Asteroids
{
    public class Bullet : GameEntity, IHasCollider
    {
        private Vector2 acceleration;
        private readonly float radius = 2f;
        private readonly float speed = 10f;
        private readonly double lifeTime = 750 * SharpRayConfig.TickMultiplier;
        private double elapsed;
        public ICollider Collider { get; }

        public Bullet(Vector2 origin, float angle, float initialForce)
        {
            Position = origin;
            acceleration = new Vector2(MathF.Cos(angle) * (speed + initialForce), MathF.Sin(angle) * (speed + initialForce));

            Collider = new CircleCollider
            {
                Center = Position,
                Radius = radius
            };
            RenderLayer = Game.RlAsteroidsBullets;
        }

        public override void Render()
        {
            DrawCircleV(Position, radius, Color.YELLOW);
            //Collider.Render();
        }

        public override void Update(double deltaTime)
        {
            elapsed += deltaTime;

            if (elapsed > lifeTime)
                EmitEvent(new BulletLifeTimeExpired { Bullet = this });

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
