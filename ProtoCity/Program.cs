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
        public static List<UIComponent> UIComponents = new()
        {
            new Circle
            {
                Id = 0,
                Position = new Vector2(Width / 2, Height / 2),
                BaseColor = RED,
                Radius = 5f
            },
            new Circle
            {
                Id = 1,
                Position = new Vector2(150, 150),
                BaseColor = YELLOW,
                Radius = 15f,
            },
            new Rectangle
            {
                Id = 2,
                Position = new Vector2(200, 300),
                BaseColor = GREEN,
                Size = new Vector2(20, 20)
            },
            new Polygon
            {
                Id = 3,
                Position = new Vector2(Width / 2, Height / 2),
                BaseColor = BLUE,
                TextCoords = Array.Empty<Vector2>(),
                Points = new Vector2[]
                    {
                    new Vector2(0,  50),
                    new Vector2(50,  50),
                    new Vector2(50, 0),
                },
            }
        };

        private static readonly Stack<IEditEvent> UndoStack = new();
        private static readonly Stack<IEditEvent> RedoStack = new();
        private static readonly List<Action> ToBeFlushed = new();

        static void Main(string[] args)
        {
            Mouse.Actions.Add(MouseEventHandler);
            KeyBoard.Actions.Add(KeyBoardEventHandler);
            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);

            while (!WindowShouldClose())
            {
                foreach (var c in UIComponents)
                    c.Update(GetMousePosition());

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                FlushUIEvents();
                Draw();
            }

            CloseWindow();
        }

        internal static void UIEventHandler(UIEvent e)
        {
            if (e is IEditEvent edit)
            {
                if (edit is DeleteEdit d)
                {
                    ToBeFlushed.Add(() => UIComponents.Remove(d.UIComponent));
                }
                UndoStack.Push(edit);
            }
        }

        private static void FlushUIEvents()
        {
            ToBeFlushed.ForEach(a => a());
            ToBeFlushed.Clear();
        }

        private static void KeyBoardEventHandler(IKeyBoardEvent e)
        {
            foreach (var comp in UIComponents)
                comp.OnKeyBoardEvent(e);

            if (e is KeyUndo && UndoStack.Count > 0)
            {
                var edit = UndoStack.Pop();
                edit.Undo();
                RedoStack.Push(edit);
            }

            if (e is KeyRedo && RedoStack.Count > 0)
            {
                var edit = RedoStack.Pop();
                edit.Redo();
                UndoStack.Push(edit);
            }
        }

        private static void MouseEventHandler(IMouseEvent e)
        {
            foreach (var comp in UIComponents)
                comp.OnMouseEvent(e);
        }


        private static Texture2D texture2D = new() { id = 1, };

        private static void Draw()
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
