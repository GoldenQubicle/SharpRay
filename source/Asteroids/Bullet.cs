using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using SharpRay.Core;

namespace Asteroids
{
    public class Bullet : GameEntity
    {
        public Vector2 Origin { get; private set; }
        public float Rotation { get; }
        public float InitialForce { get; }
        private float radius = 5f;
        private float speed = 5f;
        private double lifeTime = 350 * Config.TickMultiplier;
        private double elapsed;
        public Bullet(Vector2 origin, float rotation, float initialForce)
        {
            Origin = origin;
            Rotation = rotation;
            speed += initialForce;
            Collider = new CircleCollider { Center = Origin, Radius = radius };
        }


        public override void Render()
        {
            DrawCircleV(Origin, radius, Color.YELLOW);
            Collider.Render();
        }

        public override void Update(double deltaTime)
        {
            elapsed += deltaTime;

            if (elapsed > lifeTime)
                EmitEvent(new BulletLifeTimeExpired { Bullet = this });

            Origin += new Vector2(MathF.Cos(Rotation - MathF.PI / 2) * speed, MathF.Sin(Rotation - MathF.PI / 2) * speed);
            (Collider as CircleCollider).Center = Origin;
        }
    }
}
