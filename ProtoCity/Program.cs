using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using System.Numerics;
using System;
using Raylib_cs;
using System.Collections.Generic;

namespace ProtoCity
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;
        private static List<Entity> Entities = new()
        {
            new Circle { Id = 0, Position = new Vector2(50, 50), Radius = 25f, Color = PURPLE },
            new Circle { Id = 1, Position = new Vector2(150, 150), Radius = 15f, Color = YELLOW },
            new Rectangle { Id = 2, Position = new Vector2(200, 300), Size = new Vector2(20, 20), Color = GREEN },
        };
        static void Main(string[] args)
        {
            Mouse.Actions.Add(EntityHandler);

            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            while (!WindowShouldClose())
            {
                Mouse.DoEvents();
                Draw();
            }

            CloseWindow();
        }

        static int EntityUnderCursor = -1;
        static void EntityHandler(IMouseEvent me)
        {
            //so the issue atm is; the isOver check needs to happen continue
            //as now it still registers as being over an entity when in fact the mouse moved away this was done to allow dragging
            //so, what needs to happen; isOver needs to be checken continiusly, expect when a dragging action is started
            //otherwise the motion becomes choppy and annoying
            foreach (var e in Entities)
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

        static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);

            foreach (var e in Entities)
            {
                if (e is Circle c) DrawCircleV(c.Position, c.Radius, c.Color);

                if (e is Rectangle r) DrawRectangleV(r.Position, r.Size, r.Color);
            };

            EndDrawing();
        }
    }
}
