namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public ICollider Collider { get; }
        public string Stage { get; }
        public Vector2 Heading { get; private set; }

        private int HitPoints { get; set; }
        private Vector2 TextureOffset { get; }
        private Vector2 TexturePos { get; set; }
        private Texture2D Texture { get; }

        private float RotationAngle; //inital orientation
        private float RotationSpeed;// in radians per fixed update
        private bool HasSpawned;

        public Asteroid(Vector2 position, Vector2 heading, string stage)
        {
            Position = position;
            Texture = AsteroidManager.GetRandomAsteroidTexture(stage);
            Size = new Vector2(Texture.width, Texture.height);
            Heading = heading;
            Stage = stage;
            HitPoints = AsteroidManager.HitPoints[stage];
            TextureOffset = Size / 2;
            RotationAngle = GetRandomValue(-50, 50) / 1000f;
            RotationSpeed = GetRandomValue(-50, 50) / 1000f;
            Collider = new RectCollider { Position = Position, Size = Size };
            RenderLayer = Game.RlAsteroidsBullets;
        }

        public override void Update(double deltaTime)
        {
            Position += Heading;
            TexturePos = Vector2.Transform(Position - TextureOffset, Matrix3x2.CreateRotation(RotationAngle, Position));
            (Collider as RectCollider).Position = Position - TextureOffset;
            RotationAngle += RotationSpeed;

            Heading = Position switch
            {
                Vector2 { X: < 0 } or Vector2 { X: > Game.WindowWidth } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitX) * new Vector2(1.15f, 1.15f),
                Vector2 { Y: < 0 } or Vector2 { Y: > Game.WindowHeight } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitY) * new Vector2(1.15f, 1.15f),
                _ => Heading
            };

            HasSpawned = Position.X > 0 && Position.X < Game.WindowWidth
                      && Position.Y > 0 && Position.Y < Game.WindowHeight;

        }


        public void OnCollision(IHasCollider e)
        {
            if (e is Bullet b)
            {
                //depending on weapon type obviously
                HitPoints -= 1;

                EmitEvent(new BulletLifeTimeExpired
                {
                    Bullet = b
                });

                if (HitPoints == 0)
                    EmitEvent(new AsteroidDestroyed
                    {
                        Asteroid = this,
                        Bullet = b
                    });
            }
        }


        public override void Render()
        {
            DrawTextureEx(Texture, TexturePos, RAD2DEG * RotationAngle, 1f, Color.WHITE);

            var startPos = Position - new Vector2(Size.X / 2, -Size.Y / 2);
            var endPos = Position + new Vector2(Size.X / 2, Size.Y / 2);
            var a = MapRange(HitPoints, 0, AsteroidManager.HitPoints[Stage], 0, 1);
            var l = Vector2.Lerp(startPos, endPos, a);
            DrawLineEx(startPos, l, 5, Color.GREEN);

            //DEBUG
            //Collider.Render();
            //DrawCircleV(Position, 5, Color.DARKGREEN);
        }


        public override void OnMouseEvent(IMouseEvent e)
        {

        }
    }
}
