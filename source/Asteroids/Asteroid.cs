using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollision
    {
        public Asteroid(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;

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
            if(e is Bullet b)
            {
                EmitEvent(new BulletHitAsteroid { Bullet = b });
            }

            if(e is Ship)
            {
                EmitEvent(new ShipHitAsteroid());
            }
        }
    }
}
