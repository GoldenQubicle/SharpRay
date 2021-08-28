using Raylib_cs;
using SharpRay.Collision;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider, IHasCollision
    {
        public Collider Collider { get; }
        public int Strength { get; private set; }
        public int Stages { get; }

        private Vector2[] Points;
        private Vector2 Center;
        private float RotationAngle;//0.05f; // in radians per fixed update
        private Matrix3x2 Translation;
        private Matrix3x2 Rotation;

        public Asteroid(Vector2 position, Vector2 size, Vector2 heading, int strength, int stages)
        {
            Position = position;
            Size = size;
            Translation = Matrix3x2.CreateTranslation(heading);
            Strength = strength;
            Stages = stages;
            Center = Position + Size / 2;
            Points = GenerateShape();
            RotationAngle = GetRandomValue(-50, 50) / 1000f;

            Collider = new RectProCollider(Center, Size);
        }

        public override void Update(double deltaTime)
        {
            Position = Vector2.Transform(Position, Translation);
            Center = Vector2.Transform(Center, Translation);
            Rotation = Matrix3x2.CreateRotation(RotationAngle, Center);

            var m = Translation * Rotation;

            for (var i = 0; i < Points.Length; i++)
                Points[i] = Vector2.Transform(Points[i], m);

            (Collider as RectProCollider).Update(m);
        }

        public override void Render()
        {
            DrawLineV(Points[0], Points[Points.Length - 1], Color.YELLOW);

            for (var i = 0; i < Points.Length - 1; i++)
                DrawLineV(Points[i], Points[i + 1], Color.YELLOW);

            Collider.Render();

            DrawCircleV(Center, 5, Color.YELLOW);
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is Bullet b)
            {
                Strength -= b.Damage;

                if (Strength <= 0 && Stages == 0)
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });

                if (Strength <= 0 && Stages > 0)
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
                RotationAngle = 0f;
                Translation = Matrix3x2.Identity;
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
