using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollision
    {
        public int Strength { get; private set; }
        public int Stages { get; }

        public Asteroid(Vector2 position, Vector2 size, int strength, int stages)
        {
            Position = position;
            Size = size;

            Strength = strength;
            Stages = stages;
            Collider = new RectCollider
            {
                Position = Position,
                Size = Size
            };
        }


        public override void Render()
        {
            DrawRectangleV(Position, Size, Color.RED);

            Collider.Render();
        }

        public void OnCollision(GameEntity e)
        {
            if (e is Bullet b)
            {
                Strength -= b.Damage;

                if (Strength <= 0 && Stages == 1)
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                
                if(Strength <= 0 && Stages > 1)
                {
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                    EmitEvent(new AsteroidSpawnNew { Stages = Stages - 1, SpawnPoint = Position, Size = Size / 2 });
                }

                //damage bullet > strength and asteroid is smallest size already => remove bullet & remove asteroid
                //damage bullet > strength and asteroid is not smallest size => remove bullet & spawn additional, smaller asteroids based in current position
                //damage bullet < strength => remove bullet and descrease asteroid strength

                EmitEvent(new BulletHitAsteroid { Bullet = b });
            }

            if (e is Ship)
            {
                EmitEvent(new ShipHitAsteroid());
            }
        }
    }
}
