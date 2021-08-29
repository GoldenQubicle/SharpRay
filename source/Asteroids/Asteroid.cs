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
        public Matrix3x2 Transform { get; private set; }

        private int Strength;
        private Vector2 Heading;
        private int Stage;

        private Vector2[] Points;
        private Vector2 Center;
        private float RotationAngle;//0.05f; // in radians per fixed update
        private Matrix3x2 Translation;
        private Matrix3x2 Rotation;


        public Asteroid(Vector2 position, Vector2 size, Vector2 heading, int stage)
        {
            Position = position;
            Size = size;
            Translation = Matrix3x2.CreateTranslation(heading);
            Heading = heading;
            Stage = stage;
            Strength = stage * 5;

            Center = Position + Size / 2;
            Points = GenerateShape();
            RotationAngle = GetRandomValue(-50, 50) / 1000f;

            Collider = new RectProCollider(Center, Size);
        }

        public Asteroid(Vector2 position, Vector2 size, Matrix3x2 translation, float rotation, int stage)
        {
            Position = position;
            Size = size;
            Translation = translation;
            Stage = stage;
            Strength = stage * 5;

            Center = Position + Size / 2;
            Points = GenerateShape();
            RotationAngle = rotation;

            Collider = new RectProCollider(Center, Size);
        }


        public override void Update(double deltaTime)
        {
            Position = Vector2.Transform(Position, Translation);
            Center = Vector2.Transform(Center, Translation);
            Rotation = Matrix3x2.CreateRotation(RotationAngle, Center);

            Transform = Translation * Rotation;

            for (var i = 0; i < Points.Length; i++)
                Points[i] = Vector2.Transform(Points[i], Transform);

            (Collider as RectProCollider).Update(Transform);
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

                if (Strength <= 0 && Stage == 1)
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });

                if (Strength <= 0 && Stage > 1)
                {
                    EmitEvent(new AsteroidDestroyed { Asteroid = this });
                    EmitEvent(new AsteroidSpawnNew
                    {
                        Stage = Stage - 1,
                        Position = Position,
                        Size = Size / 2,
                        //Translation = Translation,
                        Rotation = RotationAngle,
                        Heading = Heading
                    });
                }

                EmitEvent(new BulletHitAsteroid { Bullet = b });
            }

            if (e is Ship s)
            {
                EmitEvent(new ShipHitAsteroid());
            }

            if (e is Asteroid a)
            {
                //super basic bounce mechanic, prone to some issues..
                RotationAngle = -1 * RotationAngle;
                Matrix3x2.Invert(Translation, out var inverse);
                Translation = inverse;
            }
        }

        private Vector2[] GenerateShape()
        {
            //TODO check for intersecting lines at corners
            var xpoints = Stage * 3;
            var ypoints = Stage * 2;
            var min = Stage * 2;
            var max = Stage * 10;
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
