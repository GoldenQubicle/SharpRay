using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System;
using static SharpRay.Core.Application;
using SharpRay.Utils;

namespace ProtoCity
{
    public class Edge : Entity
    {
        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 C { get; }

        public int Depth { get; set; } = 50;

        public Edge(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
            C = Vector2.Lerp(A, B, .5f);
        }

        public override void Render()
        {
            DrawLineEx(A, B, 3, Color.LIGHTGRAY);

            DrawTextV("A", A, 18, Color.BLACK); 
            DrawTextV("B", B, 18, Color.BLACK);

            var d = B - A; 
            var n1 = Vector2.Normalize(new Vector2(-d.Y, d.X)) * Depth;
            var n2 = Vector2.Normalize(new Vector2(d.Y, -d.X)) * Depth;

            DrawCircleV(C, 5, Color.PURPLE);
            DrawCircleV(C + n1, 3, Color.PINK);

            var rayOrigin = C + n1;
            var rayA = Vector2.Transform(Vector2.Normalize(A - C), Matrix3x2.CreateScale(1500f) * Matrix3x2.CreateTranslation(rayOrigin));
            var rayB = Vector2.Transform(Vector2.Normalize(B - C), Matrix3x2.CreateScale(1500f) * Matrix3x2.CreateTranslation(rayOrigin));

            DrawLineV(rayOrigin, rayA, Color.RED);
            DrawLineV(rayOrigin, rayB, Color.RED);


        }
    }
}
