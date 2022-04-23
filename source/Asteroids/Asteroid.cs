using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Interfaces;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public ICollider Collider { get; }
        public Vector2 Heading { get; }
        private Vector2 offset;
        private Vector2 texturePos;
        private int Strength;
        private int Stage;
        private readonly Texture2D texture;
        private float RotationAngle; //inital orientation
        private float RotationSpeed;// in radians per fixed update
        private Matrix3x2 Translation;
        private bool isDirty;

        public Asteroid(Vector2 position, Vector2 heading, int stage, Texture2D texture)
        {
            Position = position;
            Size = new Vector2 (texture.width, texture.height);
            Heading = heading;
            Stage = stage;
            offset = Size / 2;
            this.texture = texture;
            Strength = stage * 10;
            Translation = Matrix3x2.CreateTranslation(Heading);
            RotationAngle = GetRandomValue(-50, 50) / 1000f;
            RotationSpeed = GetRandomValue(-50, 50) / 1000f;
            Collider = new RectCollider { Position = Position, Size = Size };
        }

        public override void Update(double deltaTime)
        {
            Position = Vector2.Transform(Position, Translation);
            texturePos = Vector2.Transform(Position - offset, Matrix3x2.CreateRotation(RotationAngle, Position));
            (Collider as RectCollider).Position = Position - offset;
            RotationAngle += RotationSpeed;
        }

        public override void Render()
        {
            DrawTextureEx(texture, texturePos, RAD2DEG * RotationAngle, 1f, Color.WHITE);

            //DEBUG
            //Collider.Render();
            //DrawCircleV(Position, 5, Color.DARKGREEN);
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is Bullet b && Strength > 0)
            {
                Strength -= b.Damage;

                if (Strength <= 0 && Stage == 1)
                {
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                }

                if (Strength <= 0 && Stage > 1 && !isDirty)
                {
                    isDirty = true; //prevent emitting multiple spawn events per frame when dealing with an onslaught of bullets
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                    EmitEvent(new AsteroidSpawnNew
                    {
                        Stage = Stage - 1,
                        Position = Vector2.Lerp(Position, Position + Size / 2, .5f),
                        Heading = Heading
                    });
                }

                EmitEvent(new BulletHitAsteroid { Bullet = b });
            }

            if (e is Ship s)
            {
                s.TakeDamage(Strength);
                EmitEvent(new ShipHitAsteroid { ShipHealth = s.Health });
            }
        }


        public override void OnMouseEvent(IMouseEvent e)
        {
           
        }
    }
}
