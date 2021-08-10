using Raylib_cs;
using SharpRay.Entities;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
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

        private Vector2 Velocity;
        private bool hasThrust;
        private float maxThrust = 10;

        private double accelerateTime = 500 * Config.TickMultiplier;
        private double decelerateTime = 1500 * Config.TickMultiplier;
        private double elapsedAccelerateTime = 0d;
        private double elapsedDecelerateTime = 0d;
        private float currentAcceleration = 0f;

        private float currentRotation;

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
            //accelerate & decelerate according to easing curve
            if (hasThrust)
            {
                var t = Math.Min(elapsedAccelerateTime, accelerateTime);
                currentAcceleration = Easings.EaseQuadOut((float)t, 0f, 1f, (float)accelerateTime);
                elapsedAccelerateTime += deltaTime;
            }
            else if (currentAcceleration > 0)
            {
                var t = Math.Min(elapsedDecelerateTime, decelerateTime);
                currentAcceleration = 1 - Easings.EaseCircOut((float)t, 0f, 1f, (float)decelerateTime);
                elapsedDecelerateTime += deltaTime;
            }

            //apply thrust to position
            var thrust = currentAcceleration * maxThrust;
            Velocity = new Vector2(MathF.Cos(currentRotation - MathF.PI / 2) * thrust, MathF.Sin(currentRotation - MathF.PI / 2) * thrust);
            Position += Velocity;


            //TODO apply easing curves to rotation as well

            //apply rotation
            Points[0] = Position + new Vector2(MathF.Cos(currentRotation - MathF.PI / 2) * radius, MathF.Sin(currentRotation - MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(currentRotation - MathF.PI / 2 + phi * 2) * radius, MathF.Sin(currentRotation - MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(currentRotation - MathF.PI / 2 + phi) * radius, MathF.Sin(currentRotation - MathF.PI / 2 + phi) * radius);

            //update collider
            (Collider as CircleCollider).Center = Position;

            //bounds check
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
            currentRotation = e switch
            {
                KeyLeftDown => currentRotation -= rotationSpeed,
                KeyRightDown => currentRotation += rotationSpeed,
                _ => currentRotation
            };


            if (e is KeyUpDown && !hasThrust)
            {
                elapsedAccelerateTime = MapRange(currentAcceleration, 0d, 1d, 0d, accelerateTime);
                hasThrust = true;
            }

            if (e is KeyUpReleased && hasThrust)
            {
                elapsedDecelerateTime = MapRange(1d - currentAcceleration, 0d, 1d, 0d, decelerateTime);
                hasThrust = false;
            }

            if (e is KeySpaceBarPressed)
                EmitEvent(new ShipShootBullet { Origin = Points[0], Rotation = currentRotation, Force = currentAcceleration*maxThrust });
        }


    }
}
