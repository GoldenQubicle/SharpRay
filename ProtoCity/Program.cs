using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using Raylib_cs;
using System.Numerics;
using System.Reflection.PortableExecutable;

namespace ProtoCity
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;
        static void Main(string[] args)
        {
            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            while (!WindowShouldClose())
            {
                Draw();
            }

            CloseWindow();
        }

        static Vector2 center = new Vector2(Width / 4, Height / 4);
        static bool IsOver = false;
        static bool IsDragging = false;
        static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);

            var mPos = GetMousePosition();
            var radius = 10;

            if (mPos.X > center.X - radius && mPos.X < center.X + radius &&
                mPos.Y > center.Y - radius && mPos.Y < center.Y + radius)
                IsOver = true;
            else
                IsOver = false;

            if (IsOver && IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
                IsDragging = true;

            if (IsOver && IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                IsDragging = false;

            if (IsDragging)
            {
                center = mPos;
                if (center.X < 0) center.X = 0;
                if (center.X > Width) center.X = Width;
                if (center.Y < 0) center.Y = 0;
                if (center.Y > Height) center.Y = Height;
            }

            DrawCircleV(center, radius, GREEN);
            EndDrawing();
        }
    }
}
