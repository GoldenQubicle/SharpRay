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
        private float thrust = 0f;

        private float maxAcceleration= 20f;
        private float currentAcceleration = 0f;
        private float previousAcceleration = 0f;
        private float currentDeceleration = 0f;
        private float previousDeceleration = 0f;
        private double accelerateTime = 350 * Config.TickMultiplier;
        private double decelerateTime = 200 * Config.TickMultiplier;
        private double elapsedAccelerateTime = 0d;
        private double elapsedDecelerateTime = 0d;

        private Vector2 Velocity = new Vector2();
        private Vector2 Acceleration = new Vector2();

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
            //if player presses forward and current speed < maxSpeed, accelerate untill current speed == max speed 
            if(hasThrust && currentAcceleration < maxAcceleration && elapsedAccelerateTime < accelerateTime)
            {
                Console.WriteLine("Acccelerating");
                var e = Easings.EaseCircIn((float)elapsedAccelerateTime, 0f, maxAcceleration, (float)accelerateTime);
                Console.WriteLine($"easing: {e} | elapsed: {elapsedAccelerateTime} ");
                currentAcceleration = e - previousAcceleration;
                previousAcceleration = e;

                elapsedAccelerateTime += deltaTime;
                elapsedDecelerateTime = 0d;

                Console.WriteLine($"accelerating : {currentAcceleration}");
            }

            //if player releases forward and current speed > 0, decelerate untill current speed == 0
            if (!hasThrust && currentAcceleration > 0 && elapsedDecelerateTime < decelerateTime)
            {
                Console.WriteLine("Decelerating");

                var e = Easings.EaseCircOut((float)elapsedDecelerateTime, maxAcceleration, 0f, (float)decelerateTime);
                Console.WriteLine($"easing: {e} | elapsed: {elapsedDecelerateTime} ");

                currentAcceleration = e - previousAcceleration;
                previousAcceleration = e;

                elapsedDecelerateTime += deltaTime;
                elapsedAccelerateTime = 0d;

                Console.WriteLine($"decelarating: {currentAcceleration}");
            }


            //if player pressed left or right, and current rotation < maxRotation speed, increate rotation speed untill current rotation == maxRotation

            //if player releases left or right, and current rotation > 0, decrease rotation speed untill current rotation speed == 0


            Position += new Vector2(MathF.Cos(rotation - MathF.PI / 2) * currentAcceleration, MathF.Sin(rotation - MathF.PI / 2) * currentAcceleration);

            Points[0] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2) * radius,             MathF.Sin(rotation - MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi * 2) * radius,   MathF.Sin(rotation - MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi) * radius,       MathF.Sin(rotation - MathF.PI / 2 + phi) * radius);

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
                EmitEvent(new ShipShootBullet { Origin = Points[0], Rotation = rotation, Force = thrust });
        }
    }
}
