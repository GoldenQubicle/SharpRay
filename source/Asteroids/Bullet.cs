﻿using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Entities;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;
using SharpRay.Core;
using SharpRay.Interfaces;

namespace Asteroids
{
    public class Bullet : GameEntity, IHasCollider, IHasCollision
    {
        private Vector2 acceleration;
        private readonly float radius = 2f;
        private readonly float speed = 10f;
        private readonly double lifeTime = 1350 * SharpRayConfig.TickMultiplier;
        private double elapsed;
        public int Damage { get; } = 25;
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
            RenderLayer = 1;
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

        public void OnCollision(IHasCollider e)
        {
            if (e is Asteroid a)
            {
                EmitEvent(new BulletHitAsteroid { Bullet = this });

                if (a.Stage == 1)
                {
                    EmitEvent(new AsteroidDestroyed { Asteroid = a });
                    return;
                }

                EmitEvent(new AsteroidDestroyed { Asteroid = a });
                EmitEvent(new AsteroidSpawnNew
                {
                    Stage = a.Stage - 1,
                    Position = a.Position,
                    Heading = a.Heading
                });
            }
        }
    }
}
