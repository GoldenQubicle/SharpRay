using Raylib_cs;
using SharpRay.Entities;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using SharpRay.Core;
using System.Numerics;
using SharpRay.Collision;
using System;
using SharpRay.Components;
using System.Collections.Generic;

namespace Asteroids
{
    public class Ship : GameEntity, IHasCollider
    {
        private const float phi = 2.09439510239f;
        private const float HalfPI = MathF.PI / 2;
        private readonly Vector2[] Vertices = new Vector2[3];
        private readonly float radius;

        private readonly double accelerateTime = 500 * Config.TickMultiplier; // time it takes to reach max acceleration
        private readonly double decelerateTime = 2500 * Config.TickMultiplier; // time it takes from max acceleration to come to a stand still
        private readonly float maxAcceleration = 10;
        private float n_acceleration = 0f; //normalized 0-1
        private float acceleration = 0f;
        private bool hasAcceleration;

        private readonly double rotateInTime = 300 * Config.TickMultiplier; // time it takes to reach max rotation angle
        private readonly double rotateOutTime = 550 * Config.TickMultiplier; // time it takes from max rotation angle to come to a stand still
        private readonly float maxRotation = 3.5f * DEG2RAD; // in radians per frame, essentially
        private float n_rotation = 0f; //normalized 0-1
        private float rotation = 0f;
        private bool hasRotation;
        private string direction;

        private Dictionary<string, Easing> Motions;

        private const string Accelerate = nameof(Accelerate);
        private const string Decelerate = nameof(Decelerate);
        private const string RotateIn = nameof(RotateIn);
        private const string RotateOut = nameof(RotateOut);
        private const string Left = nameof(Left);
        private const string Right = nameof(Right);
        public Collider Collider { get; }

        public Ship(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            radius = Size.X / 2;

            Collider = new CircleCollider
            {
                Center = Position,
                Radius = radius,
                HitPoints = 16
            };

            Vertices[0] = Position + new Vector2(MathF.Cos(HalfPI) * radius, MathF.Sin(HalfPI) * radius);
            Vertices[1] = Position + new Vector2(MathF.Cos(HalfPI + phi * 2) * radius, MathF.Sin(HalfPI + phi * 2) * radius);
            Vertices[2] = Position + new Vector2(MathF.Cos(HalfPI + phi) * radius, MathF.Sin(HalfPI + phi) * radius);

            Motions = new Dictionary<string, Easing>
            {
                { Accelerate, new Easing(Easings.EaseQuadOut, accelerateTime) },
                { Decelerate, new Easing(Easings.EaseCircOut, decelerateTime, isReversed: true) },
                { RotateIn,   new Easing(Easings.EaseSineOut, rotateInTime) },
                { RotateOut,  new Easing(Easings.EaseSineIn, rotateOutTime, isReversed: true) },
            };

        }

        public override void Update(double deltaTime)
        {
            //update motions
            foreach (var m in Motions) m.Value.Update(deltaTime);

            //get normalized motion values if applicable
            if (hasAcceleration)
                n_acceleration = Motions[Accelerate].GetValue();
            else if (n_acceleration > 0)
                n_acceleration = Motions[Decelerate].GetValue();

            if (hasRotation)
                n_rotation = Motions[RotateIn].GetValue();
            else if (n_rotation > 0)
                n_rotation = Motions[RotateOut].GetValue();

            //update rotation
            var r = (n_rotation * maxRotation);
            rotation += direction == Left ? -1 * r : r;

            //update & apply acceleration to position
            acceleration = n_acceleration * maxAcceleration;
            Position += new Vector2(MathF.Cos(rotation - HalfPI) * acceleration, MathF.Sin(rotation - HalfPI) * acceleration);

            //apply rotation to ship vertices, based on position and hence last in order
            Vertices[0] = Position + new Vector2(MathF.Cos(rotation - HalfPI) * radius, MathF.Sin(rotation - HalfPI) * radius);
            Vertices[1] = Position + new Vector2(MathF.Cos(rotation - HalfPI + phi * 2) * radius, MathF.Sin(rotation - HalfPI + phi * 2) * radius);
            Vertices[2] = Position + new Vector2(MathF.Cos(rotation - HalfPI + phi) * radius, MathF.Sin(rotation - HalfPI + phi) * radius);

            //dont forget to update collider
            (Collider as CircleCollider).Center = Position;

            //bounds check
            if (Position.X < 0) Position = new Vector2(Game.WindowWidth, Position.Y);
            if (Position.X > Game.WindowWidth) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, Game.WindowHeight);
            if (Position.Y > Game.WindowHeight) Position = new Vector2(Position.X, 0);
        }

        public override void Render()
        {
            
            DrawTriangleLines(Vertices[0], Vertices[1], Vertices[2], Color.PINK);
            DrawCircleV(Vertices[0], 5, Color.PURPLE);

            //debug draw
            //DrawCircleV(Position, 5, Color.WHITE);
            //Collider.Render();
            //foreach (var p in Vertices.Select((p, i) => (p, i)))
            //    DrawText($"{p.i}", (int)p.p.X, (int)p.p.Y, 4, Color.BLACK);
        }


        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            (hasRotation, direction) = e switch
            {
                KeyLeftDown when !hasRotation => StartRotateIn(Left),
                KeyRightDown when !hasRotation => StartRotateIn(Right),
                KeyLeftReleased or KeyRightReleased => StartRotateOut(),
                _ => (hasRotation, direction)
            };

            hasAcceleration = e switch
            {
                KeyUpDown when !hasAcceleration => StartAccelerate(),
                KeyUpReleased when hasAcceleration => StartDecelerate(),
                _ => hasAcceleration
            };

            //obvisouly this will change w primary & secondary weapon

            //if (e is KeySpaceBarPressed)
            //    EmitEvent(new ShipFiredBullet { Origin = Vertices[0], Angle = rotation - HalfPI, Force = acceleration });

            if (e is KeySpaceBarDown)
            {
                EmitEvent(new ShipFiredBullet { Origin = Vertices[0], Angle = rotation - HalfPI, Force = acceleration });
            }

        }


        private (bool, string) StartRotateOut()
        {
            Motions[RotateOut].SetElapsedTime(n_rotation);
            return (false, direction);
        }

        private (bool, string) StartRotateIn(string direction)
        {
            Motions[RotateIn].SetElapsedTime(n_rotation);
            return (true, direction);
        }

        private bool StartAccelerate()
        {
            Motions[Accelerate].SetElapsedTime(n_acceleration);
            return true;
        }

        private bool StartDecelerate()
        {
            Motions[Decelerate].SetElapsedTime(n_acceleration);
            return false;
        }
    }
}
