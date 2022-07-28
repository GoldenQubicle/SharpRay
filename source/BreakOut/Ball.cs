namespace BreakOut
{
    public class Ball : Entity, IHasRender, IHasUpdate, IHasCollider, IHasCollision
    {
        public ICollider Collider { get; private set; }

        private Vector2 Heading { get; set; } = new Vector2(4, 4);
        private const float Radius = 15f;
        public Ball()
        {
            Position = new Vector2(20, 20);
            Collider = new CircleCollider
            {
                Center = Position,
                Radius = Radius,
                HitPoints = 8
            };
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is Paddle p)
            {
                foreach (var hp in (Collider as CircleCollider).GetHitPoints())
                {
                    if (p.Collider.ContainsPoint(hp.v))
                    {
                        if (hp.idx == 0 || hp.idx == 4) Heading = Vector2.Reflect(Heading, Vector2.UnitX);
                        else Heading = Vector2.Reflect(Heading, Vector2.UnitY);
                    }
                }

            }
        }

        public override void Render()
        {
            DrawCircleV(Position, Radius, Color.RAYWHITE);
            Collider.Render();

            (Collider as CircleCollider).Center = Position;
        }

        public override void Update(double deltaTime)
        {
            Heading = Position switch
            {
                Vector2 { X: < 0 + Radius } or Vector2 { X: > WindowWidth - Radius } => (Vector2.Reflect(Heading, Vector2.UnitX)),
                Vector2 { Y: < 0 + Radius } or Vector2 { Y: > WindowHeight - Radius } => (Vector2.Reflect(Heading, Vector2.UnitY)),
                _ => Heading
            };

            Position += Heading;
        }
    }
}
