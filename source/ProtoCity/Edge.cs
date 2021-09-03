using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class Edge : Entity
    {

        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 C { get; }
        public int Depth { get; set; } = 50;

        private const float rayLength = 1500f;

        public Edge(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
            C = Vector2.Lerp(A, B, .5f);
        }

        public override void Render()
        {
            DrawLineEx(A, B, 3, Color.LIGHTGRAY);

            DrawTextV("A", A, 15, Color.BLACK);
            DrawTextV("B", B - new Vector2(15, 0), 15, Color.BLACK);

            DrawRays();
        }

        private void DrawRays()
        {
            var (rayOrigin, rayA, rayB) = GetRays();

            DrawCircleV(rayOrigin, 3, Color.RED);
            DrawLineV(rayOrigin, rayA, Color.RED);
            DrawLineV(rayOrigin, rayB, Color.RED);
        }

        private (Vector2 origin, Vector2 rayA, Vector2 rayB) GetRays()
        {
            var (n1, n2) = GetNormals();
            var rayOrigin = C + n1;
            var rayA = Vector2.Transform(Vector2.Normalize(A - C), Matrix3x2.CreateScale(rayLength) * Matrix3x2.CreateTranslation(rayOrigin));
            var rayB = Vector2.Transform(Vector2.Normalize(B - C), Matrix3x2.CreateScale(rayLength) * Matrix3x2.CreateTranslation(rayOrigin));
            return (rayOrigin, rayA, rayB);
        }

        private (Vector2 n1, Vector2 n2) GetNormals()
        {
            var d = B - A;
            var n1 = Vector2.Normalize(new Vector2(-d.Y, d.X)) * Depth;
            var n2 = Vector2.Normalize(new Vector2(d.Y, -d.X)) * Depth;
            return (n1, n2);
        }
    }
}
