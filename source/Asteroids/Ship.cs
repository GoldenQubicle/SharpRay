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
        private const float thrust = .15f;


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
            UpdateShip();//call once to initialize triangle points
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


        private void UpdateShip()
        {
            if (hasThrust)
                Position += new Vector2(MathF.Cos(rotation - MathF.PI / 2) * thrust, MathF.Sin(rotation - MathF.PI / 2) * thrust);

            Points[0] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2) * radius,             MathF.Sin(rotation - MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi * 2) * radius,   MathF.Sin(rotation - MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(rotation - MathF.PI / 2 + phi) * radius,       MathF.Sin(rotation - MathF.PI / 2 + phi) * radius);

            (Collider as CircleCollider).Center = Position;
        }

        public override void Update(double deltaTime)
        {
            UpdateShip();
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
