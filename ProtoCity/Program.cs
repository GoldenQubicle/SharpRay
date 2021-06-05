using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using System.Numerics;
using System;

namespace ProtoCity
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;

        static void Main(string[] args)
        {
            Mouse.Actions.Add(e => { if (e is MouseLeftClick) Console.WriteLine("left button"); });
            Mouse.Actions.Add(e => { if (e is MouseMiddleClick) Console.WriteLine("middle button"); });
            Mouse.Actions.Add(e => { if (e is MouseRighttClick) Console.WriteLine("right button"); });
            Mouse.Actions.Add(e => { if (e is MouseLeftDrag) center = e.Position; });
            Mouse.Actions.Add(e => { if (e is MouseWheelUp) Radius += 1.5f; });
            Mouse.Actions.Add(e => { if (e is MouseWheelDown) Radius -= 1.5f; });

            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            while (!WindowShouldClose())
            {
                Mouse.DoEvents();
                Draw();
            }

            CloseWindow();
        }

        static Vector2 center = new Vector2(Width / 4, Height / 4);
        static float Radius = 10f;
        static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);

            DrawCircleV(center, Radius, GREEN);
            EndDrawing();
        }
    }
}
