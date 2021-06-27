using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static SharpRay.Config;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Numerics;

namespace SharpRay
{
    internal class Program
    {
        public static string AssestsFolder = Path.Combine(AppContext.BaseDirectory, @"assests");
        public const double TickMultiplier = 10000d;

        private static readonly List<Action> EventActions = new();
        private static readonly Stopwatch sw = new();
        private static List<Entity> Entities => new()
        {
            new Polygon
            {
                Position = new Vector2(WindowWidth / 2, WindowHeight / 2),
                ColorDefault = DARKPURPLE,
                ColorFocused = PURPLE,
                TextCoords = Array.Empty<Vector2>(),
                Points = Polygon.CreatePoints(6, 100),
                OnMouseLeftClick = e => new UIEvent { UIComponent = e }
            },
        };

        public static double MapRange(double s, double a1, double a2, double b1, double b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);

        static void Main(string[] args)
        {
            EntityEventInitialisation(Entities);
            InitAudioDevice();
            Audio.Initialize();
            
            InitWindow(WindowWidth, WindowHeight, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(GetMonitorWidth(0) / 2 + 128, GetMonitorHeight(0) / 2 - WindowHeight / 2);

            sw.Start();
            var past = 0L;

            while (!WindowShouldClose())
            {
                var delta = GetDeltaTime(ref past);

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                DoCollisions();
                DoUpdate(delta);
                DoRender();
                DoEventActions();
            }

            CloseAudioDevice();
            CloseWindow();
        }

        private static void EntityEventInitialisation(List<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e is IKeyBoardListener kbl) KeyBoard.EmitEvent += kbl.OnKeyBoardEvent;
                if (e is IMouseListener ml) Mouse.EmitEvent += ml.OnMouseEvent;
                if (e is IEventEmitter<IGameEvent> pe) SetEmitEventActions(pe, OnGameEvent, Audio.OnGameEvent);
                if (e is IEventEmitter<IUIEvent> eui) SetEmitEventActions(eui, OnUIEvent, Audio.OnUIEvent);
            }
        }

        private static void EntityEventInitialisation(params Entity[] entities) => EntityEventInitialisation(entities.ToList());

        public static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent
        {
            foreach (var action in onEventActions) e.EmitEvent += action;
        }

        private static long GetDeltaTime(ref long past)
        {
            var now = sw.ElapsedTicks;
            var delta = now - past;
            past = now;
            return delta;
        }

        private static void DoCollisions()
        {
            var gameEntities = Entities.OfType<GameEntity>().ToArray();

            for (var i = 0; i < gameEntities.Length; i++)
            {
                var e1 = gameEntities[i];
                if (e1 is IHasCollision cl1)
                {
                    for (var j = 0; j < gameEntities.Length; j++)
                    {
                        var e2 = gameEntities[j];
                        if (e1 != e2 && CheckCollisionRecs(e1.Collider, e2.Collider))
                            cl1.OnCollision(e2);
                    }
                }
            }
        }

        private static void DoUpdate(double deltaTime)
        {
            foreach (var e in Entities) e.Update(deltaTime);
        }

        private static void DoRender()
        {
            BeginDrawing();
            ClearBackground(GRAY);
            foreach (var e in Entities) e.Render();
            EndDrawing();
        }

        private static void DoEventActions()
        {
            foreach (var a in EventActions) a();
            EventActions.Clear();
        }



        public static void OnGameEvent(IGameEvent e)
        {


        }

        public static void OnUIEvent(IUIEvent e)
        {
            Console.WriteLine(e.GetType());
        }
    }
}
