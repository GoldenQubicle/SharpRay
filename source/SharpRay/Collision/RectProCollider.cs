using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using static Raylib_cs.Raylib;

namespace SharpRay.Collision
{
    public class RectProCollider : Collider
    {
        public Vector2 Center { get; private set; }
        public Vector2 Size { get; }
        public float Rotation { get; private set; }
        private Vector2[] Points = new Vector2[4];
        public RectProCollider(Vector2 center, Vector2 size, float rotation)
        {
            Center = center;
            Size = size;
            Rotation = rotation;

            Points[0] = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2);
            Points[1] = new Vector2(Center.X + Size.X / 2, Center.Y - Size.Y / 2);
            Points[2] = new Vector2(Center.X + Size.X / 2, Center.Y + Size.Y / 2);
            Points[3] = new Vector2(Center.X - Size.X / 2, Center.Y + Size.Y / 2);
        }
               
        public override void Render()
        {
            DrawLineV(Points[0], Points[1], Color);
            DrawLineV(Points[1], Points[2], Color);

            //DrawLineV(Points[2], Points[0], Color.BLUE);

            DrawLineV(Points[2], Points[3], Color);
            DrawLineV(Points[3], Points[0], Color);
        }

        public IEnumerable<(Vector2 p1, Vector2 p2, Vector2 p3)> GetTriangles()
        {
            yield return (Points[0], Points[1], Points[2]);
            yield return (Points[2], Points[3], Points[0]);
        }

        public IEnumerable<(Vector2 start, Vector2 end)> GetLines()
        {
            for (var i = 0; i < Points.Length; i++)
                yield return i == 3 ? (Points[i], Points[0]) : (Points[i], Points[i + 1]);
        }
    }
}