using static Raylib_cs.Raylib;
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

        public static List<Entity> Entities = new()
        {
            new Player
            {
                Position = new Vector2(300, 200),
                Size = new Vector2(20, 20),
                Bounds = new Vector2(Width, Height)
            },
            new FoodParticle
            {
                Position = new Vector2(Width / 2, Height / 2),
                Size = new Vector2(15, 15),
                Color = GREEN
            },
            new PoisonParticle
            {
                Position = new Vector2(100, 100),
                Size = new Vector2(15, 15),
                Color = RED
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
            //new Polygon
            //{
            //    Position = new Vector2(Width / 2, Height / 2),
            //    BaseColor = BLUE,
            //    TextCoords = Array.Empty<Vector2>(),
            //    Points = new Vector2[]
            //        {
            //            new Vector2(0, 50),
            //            new Vector2(50, 50),
            //            new Vector2(50, 0),
            //        },
            //},
            new ToggleButton
            {
                Position = new Vector2(Width - 150, 5),
                Size = new Vector2(100, 25),
                BaseColor = BEIGE,
                OnDrawText = () => ToggleButtonStopwatch.Elapsed.ToString("hh':'mm':'ss"),
                OnMouseLeftClick = tb => new ToggleTimer { UIComponent = tb, IsPaused = !(tb as ToggleButton).IsToggled }
            }
        };

        private static Stopwatch ToggleButtonStopwatch = new();

        public const string AssestsFolder = @"C:\Users\Erik\source\repos\SharpRayEngine\assests";
        public const double TickMultiplier = 10000d;
        private static readonly Stack<IHasUndoRedo> UndoStack = new();
        private static readonly Stack<IHasUndoRedo> RedoStack = new();
        private static readonly List<Action> EventActions = new();
        
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
                if (e is IEventEmitter<IPlayerEvent> pe) pe.EmitEvent += OnPlayerEvent;
            }

            var gameEntities = Entities.OfType<GameEntity>().ToArray();

            InitAudioDevice();
            Audio.Initialize();
            SetTargetFPS(60);
            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(1366, 712);

            var sw = new Stopwatch();
            sw.Start();
            var past = sw.ElapsedTicks;

            while (!WindowShouldClose())
            {
                var now = sw.ElapsedTicks;
                var delta = now - past; 
                past = now;

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                DoCollisions(gameEntities);
                DoRender(delta);
                DoEventActions();
            }

            CloseAudioDevice();
            CloseWindow();
        }

        public static double MapRange(double s, double a1, double a2, double b1, double b2) 
        {
            return b1 + ((s - a1) * (b2 - b1)) / (a2 - a1);
        }

        private static void DoCollisions(GameEntity[] gameEntities)
        {
            var geLength = gameEntities.Length;
            for (var i = 0; i < geLength; i++)
            {
                var e1 = gameEntities[i];
                if (e1 is IHasCollision cl1)
                {
                    for (var j = 0; j < geLength; j++)
                    {
                        var e2 = gameEntities[j];
                        if (e1 != e2 && CheckCollisionRecs(e1.Collider, e2.Collider))
                            cl1.OnCollision(e2);
                    }
                }
            }
        }

        private static void OnPlayerEvent(IPlayerEvent e)
        {
            if (e is PlayerConsumedParticle cp)
                EventActions.Add(() => Entities.Remove(cp.GameEntity));
        }

        private static void OnUIEvent(IUIEvent e)
        {
            if (e is IHasUndoRedo ur) 
                UndoStack.Push(ur);

            if (e is DeleteEdit edit)
                EventActions.Add(() => Entities.Remove(edit.UIComponent));

            if (e is ToggleTimer t)
            {
                if (!t.IsPaused) ToggleButtonStopwatch.Start();
                if (t.IsPaused) ToggleButtonStopwatch.Stop();
            }

            if (e is RectangleLeftClick)
                Console.WriteLine(e.UIComponent.GetType().Name);
        }

        private static void DoEventActions()
        {
            EventActions.ForEach(a => a());
            EventActions.Clear();
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

        private static void DoRender(double delta)
        {
            BeginDrawing();
            ClearBackground(GRAY);

            foreach (var e in Entities)
                e.Render(delta);

            EndDrawing();
        }
    }
}
