using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using System.Numerics;
using System;
using Raylib_cs;
using System.Collections.Generic;
using System.Linq;

namespace ProtoCity
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;
        private static List<UIComponent> UIComponents = new()
        {
            new Circle { Id = 0, Position = new Vector2(Width/2, Height/2), Color = RED, Radius = 5f },
            new Circle { Id = 1, Position = new Vector2(150, 150), Color = YELLOW, Radius = 15f },
            new Rectangle { Id = 2, Position = new Vector2(200, 300), Color = GREEN, Size = new Vector2(20, 20) },
            new Polygon
            {
                Id = 3,
                Position = new Vector2(Width / 2, Height / 2),
                Color = BLUE,
                TextCoords = Array.Empty<Vector2>(),
                Points = new Vector2[]
                    {
                    new Vector2(0,  50),
                    new Vector2(50,  50),
                    new Vector2(50, 0),                    
                },
            }
        };

        static void Main(string[] args)
        {
            Mouse.Actions.Add(UICOmponentHandler);

            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            while (!WindowShouldClose())
            {
                Mouse.DoEvents();
                Draw();
            }

            CloseWindow();
        }

        static int EntityUnderCursor = -1;
        static void UICOmponentHandler(IMouseEvent me)
        {
            foreach(var poly in UIComponents.OfType<Polygon>())
            {
                
            }

            foreach (var e in UIComponents)
            {
                if (e is not Circle c) continue;

                var d = Vector2.Distance(c.Position, me.Position);

                if (d < c.Radius)
                    EntityUnderCursor = c.Id;

                var isOver = EntityUnderCursor == c.Id;

                if (isOver && me is MouseLeftDrag)
                    c.Position = me.Position;

                if (isOver && me is MouseLeftRelease)
                    EntityUnderCursor = -1;

                if (isOver && me is MouseWheelUp)
                    c.Radius += 1.5f;

                if (isOver && me is MouseWheelDown)
                    c.Radius -= 1.5f;
            }
        }


        static Texture2D texture2D = new() { id = 1, };

        static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);


            foreach (var e in UIComponents)
            {
                if (e is Circle c)
                    DrawCircleV(c.Position, c.Radius, c.Color);

                if (e is Rectangle r)
                    DrawRectangleV(r.Position, r.Size, r.Color);

                if (e is Polygon p)
                    DrawTexturePoly(texture2D, p.Position, p.Points, p.TextCoords, p.Points.Length, p.Color);
            };

            EndDrawing();
        }
    }
}
