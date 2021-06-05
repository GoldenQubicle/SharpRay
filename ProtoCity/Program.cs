using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System;

namespace ProtoCity
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;

        static List<Action<MouseEvent>> MouseSubscribers = new List<Action<MouseEvent>>();

        static void Main(string[] args)
        {
            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            MouseSubscribers.Add(e => { if (e is MouseLeftClick) Console.WriteLine("left button"); });
            MouseSubscribers.Add(e => { if (e is MouseMiddleClick) Console.WriteLine("middle button"); });
            MouseSubscribers.Add(e => { if (e is MouseRighttClick) Console.WriteLine("right button"); });
            MouseSubscribers.Add(e => { if (e is MouseLeftDrag) Console.WriteLine($"dragging left {e.Position}"); });

            while (!WindowShouldClose())
            {
                DoMouseEvents();
                Draw();
            }

            CloseWindow();
        }

        static bool IsDragging = false;
        static Vector2 PreviousMousePostion;
        static void DoMouseEvents()
        {
            var currentMousePostion = GetMousePosition();
            IsDragging = currentMousePostion != PreviousMousePostion;

            //click events
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                MouseSubscribers.ForEach(ms => ms(new MouseLeftClick(currentMousePostion)));

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                MouseSubscribers.ForEach(ms => ms(new MouseMiddleClick(currentMousePostion)));

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                MouseSubscribers.ForEach(ms => ms(new MouseRighttClick(currentMousePostion)));

            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && IsDragging)
                MouseSubscribers.ForEach(ms => ms(new MouseLeftDrag(currentMousePostion)));

            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON) && IsDragging)
                IsDragging = false;

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && IsDragging)
                MouseSubscribers.ForEach(ms => ms(new MouseMiddleDrag(currentMousePostion)));

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON) && IsDragging)
                IsDragging = false;

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && IsDragging)
                MouseSubscribers.ForEach(ms => ms(new MouseRightDrag(currentMousePostion)));

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON) && IsDragging)
                IsDragging = false;


            PreviousMousePostion = currentMousePostion;
        }

        static Vector2 center = new Vector2(Width / 4, Height / 4);

        static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);
            var radius = 10;
          
            DrawCircleV(center, radius, GREEN);
            EndDrawing();
        }
    }
}
