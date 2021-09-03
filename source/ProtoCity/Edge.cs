using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System;

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
            
            //DrawCircleV(C, 3, Color.BROWN);
            
            var d = B - A;
            var n1 = Vector2.Normalize(new Vector2(-d.Y, d.X)) * Depth;
            var n2 = Vector2.Normalize(new Vector2(d.Y, -d.X)) * Depth;
            
            //DrawLineV(C, C + n1, Color.GREEN);
            //DrawLineV(C, C + n2, Color.WHITE);

            DrawLineEx(A + n1, B + n1, 1,  Color.LIGHTGRAY);
        } 
    }
}
