using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Reflection;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpRay
{
    class Program
    {
        private const int Width = 800;
        private const int Height = 480;

        public static List<Entity> Entities = new()
        {
            new Player
            {
                Position = new Vector2(300, 200),
                Size = new Vector2(15, 15),
                Bounds = new Vector2(Width, Height)
            },
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

        private static readonly Stack<IHasUndoRedo> UndoStack = new();
        private static readonly Stack<IHasUndoRedo> RedoStack = new();
        private static readonly List<Action> ToBeFlushed = new();
        private static Stopwatch Stopwatch = new();
        
        public const string AssestsFolder = @"C:\Users\Erik\source\repos\SharpRayEngine\assests";

        static void Main(string[] args)
        {
            Mouse.EmitEvent += OnMouseEvent;
            KeyBoard.EmitEvent += OnKeyBoardEvent;

            foreach (var e in Entities)
            {
                if (e is IKeyBoardListener kbl) KeyBoard.EmitEvent += kbl.OnKeyBoardEvent;
                if (e is IMouseListener ml) Mouse.EmitEvent += ml.OnMouseEvent;
                if (e is IEventEmitter<IUIEvent> ui) ui.EmitEvent += OnUIEvent;
                if (e is IEventEmitter<IAudioEvent> au) au.EmitEvent += Audio.OnAudioEvent;
            }

            InitAudioDevice();
            Audio.Initialize();

            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);
            
            SetWindowPosition(1366, 712);

            while (!WindowShouldClose())
            {
                Mouse.DoEvents();
                KeyBoard.DoEvents();
                FlushUIEvents();
                Draw();
            }

            CloseAudioDevice();
            CloseWindow();
        }

        internal static void OnUIEvent(IUIEvent e)
        {
            if (e is IHasUndoRedo ur) UndoStack.Push(ur);

            if (e is DeleteEdit edit)
                ToBeFlushed.Add(() => Entities.Remove(edit.UIComponent));

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

        private static void OnKeyBoardEvent(IKeyBoardEvent kbe)
        {
            if (kbe is KeyUndo && UndoStack.Count > 0)
            {
                var edit = UndoStack.Pop();
                edit.Undo();
                RedoStack.Push(edit);
            }

            if (kbe is KeyRedo && RedoStack.Count > 0)
            {
                var edit = RedoStack.Pop();
                edit.Redo();
                UndoStack.Push(edit);
            }
        }

        private static void OnMouseEvent(IMouseEvent me)
        {

        }

        private static void Draw()
        {
            BeginDrawing();
            ClearBackground(GRAY);

            foreach (var e in Entities)
            {
                e.Draw();
                if (e is ILoop l) l.Update(GetMousePosition());
            }

            EndDrawing();
        }
    }
}
