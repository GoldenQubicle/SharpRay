using Raylib_cs;
using SharpRay.Entities;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using SharpRay.Core;
using System.Numerics;
using SharpRay.Collision;
using System;
using System.Linq;

namespace Asteroids
{
    public class Ship : GameEntity
    {
        private const float phi = 2.09439510239f;
        private const float rotationSpeed = .001f;
        private readonly Vector2[] Points = new Vector2[3];
        private readonly float radius;
        private float rotation;
        private bool hasThrust;

        private float maxAcceleration = 10f;
        private float currentAcceleration = 0f;
        private double accelerateTime = 500 * Config.TickMultiplier;
        private double decelerateTime = 1500 * Config.TickMultiplier;
        private double elapsedAccelerateTime = 0d;
        private double elapsedDecelerateTime = 0d;

        private Vector2 Velocity = new Vector2();

        public Ship(Vector2 size, Vector2 pos)
        {
            Size = size;
            Position = pos;
            radius = Size.X / 2;
            Collider = new CircleCollider
            {
                Center = Position,
                Radius = radius
            };
            Points[0] = Position + new Vector2(MathF.Cos(MathF.PI / 2) * radius, MathF.Sin(MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(MathF.PI / 2 + phi * 2) * radius, MathF.Sin(MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(MathF.PI / 2 + phi) * radius, MathF.Sin(MathF.PI / 2 + phi) * radius);
        }

        public override void Render()
        {
            DrawCircleV(Position, 5, Color.WHITE);
            DrawTriangleLines(Points[0], Points[1], Points[2], Color.PINK);

            Collider.Render();

            DrawCircleV(Points[0], 5, Color.PURPLE);

            foreach (var p in Points.Select((p, i) => (p, i)))
                DrawText($"{p.i}", (int)p.p.X, (int)p.p.Y, 4, Color.BLACK);
        }


        private void UpdateShip(double deltaTime)
        {
            if (hasThrust)
            {
                if (currentAcceleration < maxAcceleration)
                {
                    var t = Math.Min(elapsedAccelerateTime, accelerateTime);
                    currentAcceleration = Easings.EaseQuadOut((float)t, 0f, maxAcceleration, (float) accelerateTime);
                    elapsedAccelerateTime += deltaTime;
                }
                else
                {
                    currentAcceleration = maxAcceleration;
                    elapsedAccelerateTime = 0d;
                }
            }
            else
            {
                if (currentAcceleration > 0)
                {
                    var t = Math.Min(elapsedDecelerateTime, decelerateTime);
                    var e = Easings.EaseCircOut((float)t, 0f, maxAcceleration, (float)decelerateTime);
                    currentAcceleration = Application.MapRange(e, 0f, maxAcceleration, maxAcceleration, 0f);
                    elapsedDecelerateTime += deltaTime;
                }
                else
                {
                    currentAcceleration = 0f;
                    elapsedDecelerateTime = 0d;
                }
            }

            //if player pressed left or right, and current rotation < maxRotation speed, increate rotation speed untill current rotation == maxRotation

            //if player releases left or right, and current rotation > 0, decrease rotation speed untill current rotation speed == 0


            Velocity = new Vector2(MathF.Cos(rotation - MathF.PI / 2) * currentAcceleration, MathF.Sin(rotation - MathF.PI / 2) * currentAcceleration);

            Position += Velocity;

            Points[0] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2) * radius, MathF.Sin(rotation - MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi * 2) * radius, MathF.Sin(rotation - MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi) * radius, MathF.Sin(rotation - MathF.PI / 2 + phi) * radius);

            (Collider as CircleCollider).Center = Position;


            if (Position.X < 0) Position = new Vector2(Game.WindowWidth, Position.Y);
            if (Position.X > Game.WindowWidth) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, Game.WindowHeight);
            if (Position.Y > Game.WindowHeight) Position = new Vector2(Position.X, 0);
        }

        public override void Update(double deltaTime)
        {
            UpdateShip(deltaTime);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            rotation = e switch
            {
                KeyLeftDown => rotation -= rotationSpeed,
                KeyRightDown => rotation += rotationSpeed,
                _ => rotation
            };

            hasThrust = e switch
            {
                KeyUpDown => true,
                KeyUpReleased => false,
                _ => hasThrust
            };

            if (e is KeySpaceBarPressed)
                EmitEvent(new ShipShootBullet { Origin = Points[0], Rotation = rotation, Force = currentAcceleration });
        }
    }
}
