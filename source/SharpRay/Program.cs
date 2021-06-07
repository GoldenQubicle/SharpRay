﻿using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpRay
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;
        public static List<UIComponent> UIComponents = new()
        {
            new Circle
            {
                Position = new Vector2(Width / 2, Height / 2),
                BaseColor = RED,
                Radius = 50f,
                OnRightMouseClick = e => Console.WriteLine("")
            },
            new Circle
            {
                Position = new Vector2(150, 150),
                BaseColor = YELLOW,
                Radius = 15f,
            },
            new Rectangle
            {
                Position = new Vector2(200, 300),
                BaseColor = GREEN,
                Size = new Vector2(20, 20),
                OnMouseLeftClick = r => new RectangleLeftClick { UIComponent = r }
            },
            new Polygon
            {
                Position = new Vector2(Width / 2, Height / 2),
                BaseColor = BLUE,
                TextCoords = Array.Empty<Vector2>(),
                Points = new Vector2[]
                    {
                        new Vector2(0, 50),
                        new Vector2(50, 50),
                        new Vector2(50, 0),
                    },
            },
            new ToggleButton
            {
                Position = new Vector2(Width - 150, 5),
                Size = new Vector2(100, 25),
                BaseColor = BEIGE,
                OnUpdate = () => Stopwatch.Elapsed.ToString("hh':'mm':'ss"),
                OnMouseLeftClick = tb => new ToggleTimer { UIComponent = tb, IsPaused = !(tb as ToggleButton).IsToggled }
            }
        };

        public static List<Entity> Entities = new()
        {
            new Player
            {
                Position = new Vector2(300, 200),
                Size = new Vector2(15, 15)
            }
        };

        private static readonly Stack<IHasUndoRedo> UndoStack = new();
        private static readonly Stack<IHasUndoRedo> RedoStack = new();
        private static readonly List<Action> ToBeFlushed = new();
        private static Stopwatch Stopwatch = new();

        static void Main(string[] args)
        {
            Mouse.EmitEvent += MouseEventHandler;
            KeyBoard.EmitEvent += KeyBoardEventHandler;

            foreach (var e in Entities)
            {
                if (e is IKeyBoardListener) KeyBoard.EmitEvent += e.OnKeyBoardEvent;
                if (e is IMouseListener) Mouse.EmitEvent += e.OnMouseEvent;
            }
            foreach (var c in UIComponents)
            {
                if (c is IKeyBoardListener) KeyBoard.EmitEvent += c.OnKeyBoardEvent;
                if (c is IMouseListener) Mouse.EmitEvent += c.OnMouseEvent;
                if (c is IEventEmitter<IUIEvent> e) e.EmitEvent += UIEventHandler;
            }


            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(1366, 712);

            while (!WindowShouldClose())
            {
                foreach (var comp in UIComponents)
                    comp.Update(GetMousePosition());

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                FlushUIEvents();
                Draw();
            }

            CloseWindow();
        }

        internal static void UIEventHandler(IUIEvent e)
        {
            if (e is IHasUndoRedo ur) UndoStack.Push(ur);

            if (e is DeleteEdit edit)
                ToBeFlushed.Add(() => UIComponents.Remove(edit.UIComponent));

            if (e is ToggleTimer t)
            {
                if (!t.IsPaused) Stopwatch.Start();
                if (t.IsPaused) Stopwatch.Stop();
            }

            if (e is RectangleLeftClick)
                Console.WriteLine(e.UIComponent.GetType().Name);
        }

        private static void FlushUIEvents()
        {
            ToBeFlushed.ForEach(a => a());
            ToBeFlushed.Clear();
        }

        private static void KeyBoardEventHandler(IKeyBoardEvent e)
        {
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

        }

        private static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);

            foreach (var e in Entities)
                e.Draw();

            foreach (var comp in UIComponents)
                comp.Draw();

            EndDrawing();
        }
    }
}
