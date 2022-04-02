using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using SharpRay.Core;

namespace Asteroids
{
    public class Bullet : GameEntity, IHasCollider
    {
        private Vector2 acceleration;
        private readonly float radius = 2f;
        private readonly float speed = 10f;
        private readonly double lifeTime = 1350 * SharpRayConfig.TickMultiplier;
        private double elapsed;
        public int Damage { get; } = 25;
        public Collider Collider { get; }

        public Bullet(Vector2 origin, float angle, float initialForce)
        {
            Position = origin;
            acceleration = new Vector2(MathF.Cos(angle) * (speed + initialForce), MathF.Sin(angle) * (speed + initialForce));

            Collider = new CircleCollider
            { 
                Center = Position, 
                Radius = radius
            };
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
        }
    }
}
