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
           
        };

        public const string AssestsFolder = @"C:\Users\Erik\source\repos\SharpRayEngine\assests";
        public const double TickMultiplier = 10000d;
        
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
           
        }

     

        private static void OnKeyBoardEvent(IKeyBoardEvent kbe)
        {
            

        }

        private static void OnMouseEvent(IMouseEvent me)
        {

        }

        private static void DoEventActions()
        {
            EventActions.ForEach(a => a());
            EventActions.Clear();
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
