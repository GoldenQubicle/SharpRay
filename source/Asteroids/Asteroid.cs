namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public new enum Size
        {
            Tiny = 1,
            Small = 2,
            Medium = 3,
            Large = 4,
            Big = 5
        }

        public enum Type
        {
            Dirt,
            Stone,
            Emerald,
            Ruby,
            Saphire,
        }

        public ICollider Collider { get; }
        public Vector2 Heading { get; private set; }
        public Size aSize { get; }
        public Type aType { get; }
        
        private Vector2 TextureOffset { get; }
        private Vector2 TexturePos { get; set; }
        private Texture2D Texture { get; }

        private float RotationAngle;  //inital orientation
        private float RotationSpeed; // in radians per fixed update
        private bool HasSpawned;
        private int damage;

        public Asteroid(Size size, Type type, Vector2 position, Vector2 heading)
        {
            aSize = size;
            aType = type;
            Texture = GetAsteroidTexture(size, type);

            base.Size = new Vector2(Texture.width, Texture.height) * GetScale(size);
            TextureOffset = base.Size / 2;

            Position = position;
            Heading = heading;
            RotationAngle = GetRandomValue(-50, 50) / 1000f;
            RotationSpeed = GetRandomValue(-50, 50) / 1000f;
            Collider = new RectCollider { Position = Position, Size = base.Size };
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

                if (damage >= GetHitPoints(aSize, aType))
                    EmitEvent(new AsteroidDestroyed
                    {
                        Asteroid = this,
                        Bullet = b
                    });
            }
        }


        public override void Render()
        {
            DrawTextureEx(Texture, TexturePos, RAD2DEG * RotationAngle, GetScale(aSize), GetColor(aType));

            var startPos = Position - new Vector2(base.Size.X / 2, -base.Size.Y / 2);
            var endPos = Position + new Vector2(base.Size.X / 2, base.Size.Y / 2);
            var a = MapRange(damage, GetHitPoints(aSize, aType), 0, 0, 1);
            var l = Vector2.Lerp(startPos, endPos, a);
            DrawLineEx(startPos, l, 5, Color.GREEN);

            //DEBUG
            Collider.Render();
            //DrawCircleV(Position, 5, Color.DARKGREEN);
        }

       

        private static Color GetColor(Type type) => type switch
        {
            Type.Dirt or Type.Stone => Color.WHITE,
            Type.Ruby => Color.MAROON,
            Type.Emerald => Color.LIME,
            Type.Saphire => Color.GOLD,
        };

        private static float GetScale(Size size) => size switch
        {
            Size.Large => 0.75f,
            _ => 1f
        };

      
        public static int GetHitPoints(Size size, Type type) => (size, type) switch
        {
            (_, Type.Dirt) => (int)size,
            (_, Type.Stone) => (int)size * 2,
            (_, Type.Emerald) => (int)size * 3,
            (_, Type.Ruby) => (int)size * 4,
            (_, Type.Saphire) => (int)size * 5
        };


        public static int GetTotalHitPoints(Size size, Type type) =>
            GetHitPoints(size, type) + Asteroid.GetSpawns(size, type).Sum(t => GetHitPoints(t.Size, t.Type));

        public static List<(Size Size, Type Type)> GetSpawns(Size size, Type type) => (size, type) switch
        {
            (_, Type.Dirt) => size switch
            {
                Size.Big => new()
                {
                    (Size.Large, Type.Dirt),
                    (Size.Medium, Type.Dirt),
                    (Size.Large, Type.Dirt),
                    (Size.Medium, Type.Dirt),
                    (Size.Large, Type.Dirt),
                },
                Size.Large => new()
                {
                    (Size.Medium, Type.Dirt),
                    (Size.Small, Type.Dirt),
                    (Size.Medium, Type.Dirt),
                    (Size.Small, Type.Dirt),
                    (Size.Medium, Type.Dirt),

                },
                Size.Medium => new()
                {
                    (Size.Small, Type.Dirt),
                    (Size.Small, Type.Dirt),
                    (Size.Tiny, Type.Dirt),
                },
                Size.Small => new()
                {
                    (Size.Tiny, Type.Dirt),
                    (Size.Tiny, Type.Dirt),
                },
                Size.Tiny => new() { },
            },

            (_, Type.Stone) => size switch
            {
                Size.Big => new()
                {
                    (Size.Large, Type.Stone),
                    (Size.Medium, Type.Dirt),
                    (Size.Large, Type.Stone),
                    (Size.Medium, Type.Dirt),
                    (Size.Large, Type.Stone),
                },
                Size.Large => new()
                {
                    (Size.Medium, Type.Stone),
                    (Size.Small, Type.Dirt),
                    (Size.Medium, Type.Stone),
                    (Size.Small, Type.Dirt),
                    (Size.Medium, Type.Stone),

                },
                Size.Medium => new()
                {
                    (Size.Small, Type.Stone),
                    (Size.Small, Type.Dirt),
                    (Size.Tiny, Type.Stone),
                },
                Size.Small => new()
                {
                    (Size.Tiny, Type.Stone),
                    (Size.Tiny, Type.Dirt),
                },
                Size.Tiny => new()
                {
                    (Size.Tiny, Type.Dirt),
                    (Size.Tiny, Type.Dirt),
                },
            }

        };

    }
}
