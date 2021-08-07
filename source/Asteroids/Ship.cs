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
        private float rotation;
        private Vector2[] Points = new Vector2[3];
        private float radius;
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
            RotateShip();//call once to initialize triangle points
        }

        public override void Render()
        {
            DrawCircleV(Position, 20, Color.WHITE);
            DrawTriangleLines(Points[0], Points[1], Points[2], Color.PINK);

            Collider.Render();

            foreach (var p in Points.Select((p, i) => (p, i)))
                DrawText($"{p.i}", (int)p.p.X, (int)p.p.Y, 4, Color.BLACK);

            var d = Position + new Vector2(MathF.Cos(-MathF.PI/2 + rotation) * radius, MathF.Sin(-MathF.PI /2 + rotation) * radius);
            DrawCircleV(d, 5, Color.PURPLE);
        }

        private void RotateShip()
        {
            var phi = MathF.Tau / 3;

            Points[0] = Position + new Vector2(MathF.Cos(rotation-MathF.PI / 2) * radius,           MathF.Sin(rotation-MathF.PI / 2) * radius);
            Points[1] = Position + new Vector2(MathF.Cos(rotation-MathF.PI / 2 + phi * 2) * radius, MathF.Sin(rotation-MathF.PI / 2 + phi * 2) * radius);
            Points[2] = Position + new Vector2(MathF.Cos(rotation-MathF.PI / 2 + phi) * radius,     MathF.Sin(rotation-MathF.PI / 2 + phi) * radius);
        }

        public override void Update(double deltaTime)
        {
            RotateShip();
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            rotation = e switch
            {
                KeyLeft => rotation -= .001f,
                KeyRight => rotation += .001f,
                _ => rotation
            };
        }
    }
}
