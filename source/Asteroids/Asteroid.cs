using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public Collider Collider { get; }
        public int Strength { get; private set; }
        public int Stages { get; }

        private Vector2[] Points;
        private Vector2 Center;
        private float RotationAngle = 0.05f; // in radians per rotation time
        private Matrix3x2 RotationMatrix;

        public Asteroid(Vector2 position, Vector2 size, int strength, int stages)
        {
            Position = position;
            Size = size;
            Strength = strength;
            Stages = stages;
            Center = Position + Size / 2;
            Points = GenerateShape();
            RotationMatrix = Matrix3x2.CreateRotation(RotationAngle, Center);
            Collider = new RectProCollider(Center, Size, 0f);
        }


        public override void Update(double deltaTime)
        {
            for (var i = 0; i < Points.Length; i++)
            {
                Points[i] = Vector2.Transform(Points[i], RotationMatrix);
            }

            (Collider as RectProCollider).Update(RotationMatrix);
        }

        public override void Render()
        {

            DrawLineV(Points[0], Points[Points.Length - 1], Color.YELLOW);

            for (var i = 0; i < Points.Length - 1; i++)
            {
                DrawLineV(Points[i], Points[i + 1], Color.YELLOW);
            }

            //DrawRectangleV(Position, Size, Color.BLANK);
            //DrawCircleV(Center, 15, Color.YELLOW);
            Collider.Render();
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is Bullet b)
            {
                Strength -= b.Damage;

                if (Strength <= 0 && Stages == 1)
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });

                if (Strength <= 0 && Stages > 1)
                {
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                    EmitEvent(new AsteroidSpawnNew { Stages = Stages - 1, SpawnPoint = Position, Size = Size / 2 });
                }

                EmitEvent(new BulletHitAsteroid { Bullet = b });
            }

            if (e is Ship s)
            {
                EmitEvent(new ShipHitAsteroid());
            }

            if (e is Asteroid a)
            {
                //TODO bounce of each other
            }
        }

        private Vector2[] GenerateShape()
        {
            //TODO check for intersecting lines at corners
            var xpoints = Stages * 3;
            var ypoints = Stages * 2;
            var min = Stages * 2;
            var max = Stages * 10;
            var shape = new List<Vector2>();
            var xinterval = (int)Size.X / xpoints;
            var yinterval = (int)Size.Y / ypoints;

            for (var x = 0; x < xpoints; x++)
            {
                var point = Position + new Vector2((xinterval * x) + GetRandomValue(min, xinterval), GetRandomValue(min, max));
                shape.Add(point);
            }

            for (var y = 0; y < ypoints; y++)
            {
                var point = Position + new Vector2(Size.X - GetRandomValue(min, max), (yinterval * y) + GetRandomValue(min, yinterval));
                shape.Add(point);
            }

            for (var x = 0; x < xpoints; x++)
            {
                var point = Position + new Vector2((xinterval * (xpoints - x - 1)) + GetRandomValue(min, xinterval), Size.Y - GetRandomValue(min, max));
                shape.Add(point);
            }

            for (var y = 0; y < ypoints; y++)
            {
                var point = Position + new Vector2(GetRandomValue(min, max), (yinterval * (ypoints - y - 1)) + GetRandomValue(min, yinterval));
                shape.Add(point);
            }

            return shape.ToArray();
        }


        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseLeftClick)
            {
                Points = GenerateShape();
            }
        }
    }
}
