namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public ICollider Collider { get; }
        public Vector2 Heading { get; private set; }
        public int HitPoints { get; }
        public AsteroidSize aSize { get; }
        public AsteroidType aType { get; }

        private Vector2 TextureOffset { get; }
        private Vector2 TexturePos { get; set; }
        private Texture2D Texture { get; }

        private float RotationAngle;  //inital orientation
        private float RotationSpeed; // in radians per fixed update
        private bool HasSpawned;
        private Color color;
        private readonly float scale;
        private int damage;

        public Asteroid(AsteroidSize size, AsteroidType type, Vector2 position, Vector2 heading)
        {
            aSize = size;
            aType = type;
            var rd = AsteroidManager.GetRenderData(size, type);
            color = rd.color;
            scale = rd.scale;
            Texture = rd.texture;

            Size = new Vector2(Texture.width, Texture.height) * scale;
            TextureOffset = Size / 2;


            HitPoints = AsteroidManager.GetHitPoints(size, type);
            Position = position;
            Heading = heading;
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

            //TODO make reflection speed & max speed depending on asteroid size & game level
            var bounciness = BouncyLimitReached() ? Vector2.One : new Vector2(1.05f, 1.05f);
            bool BouncyLimitReached() => Math.Abs(Heading.X) > 10 || Math.Abs(Heading.Y) > 10;

            Heading = Position switch
            {
                Vector2 { X: < 0 } or Vector2 { X: > Game.WindowWidth } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitX) * bounciness,
                Vector2 { Y: < 0 } or Vector2 { Y: > Game.WindowHeight } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitY) * bounciness,
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
                damage++;

                EmitEvent(new BulletLifeTimeExpired
                {
                    Bullet = b
                });

                if (damage >= HitPoints)
                    EmitEvent(new AsteroidDestroyed
                    {
                        Asteroid = this,
                        Bullet = b
                    });
            }
        }


        public override void Render()
        {
            DrawTextureEx(Texture, TexturePos, RAD2DEG * RotationAngle, scale, color);

            var startPos = Position - new Vector2(Size.X / 2, -Size.Y / 2);
            var endPos = Position + new Vector2(Size.X / 2, Size.Y / 2);
            var a = MapRange(damage, HitPoints, 0, 0, 1);
            var l = Vector2.Lerp(startPos, endPos, a);
            DrawLineEx(startPos, l, 5, Color.GREEN);

            //DEBUG
            Collider.Render();
            //DrawCircleV(Position, 5, Color.DARKGREEN);
        }


        public override void OnMouseEvent(IMouseEvent e)
        {

        }
    }
}
