using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static SharpRay.Core.SharpRayConfig;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using SharpRay.Collision;
using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Gui;
using SharpRay.Listeners;
using System.Numerics;
using Raylib_cs;

namespace SharpRay.Core
{
    public class Application
    {
        private static readonly Mouse Mouse = new();
        private static readonly KeyBoard KeyBoard = new();
        private static readonly List<Entity> Entities = new();
        private static readonly Stopwatch sw = new();
        private static readonly List<Action> EventActions = new();
        private static readonly Stack<IHasUndoRedo> UndoStack = new();
        private static readonly Stack<IHasUndoRedo> RedoStack = new();
        public static string AssestsFolder = Path.Combine(AppContext.BaseDirectory, @"assets");
        private static long FrameCount;
        private static bool DoEventLogging;

        #region public api

        public static void Initialize(SharpRayConfig config)
        {
            DoEventLogging = config.DoEventLogging;
            Mouse.EmitEvent += OnMouseEvent;
            KeyBoard.EmitEvent += OnKeyBoardEvent;

            InitAudioDevice();
            Audio.Initialize();
            InitWindow(config.WindowWidth, config.WindowHeight, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(GetMonitorWidth(0) / 2 + 128, GetMonitorHeight(0) / 2 - config.WindowHeight / 2);

            //SetTargetFPS(60);
        }
        public static void Run()
        {
            sw.Start();
            var previous = 0L;

            while (!WindowShouldClose())
            {
                FrameCount++;
                DrawFPS(0, 0);
                var frameTime = GetFrameTime(ref previous);

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                DoCollisions();
                DoFixedUpdate(frameTime);
                DoRender();
                DoEventActions();
            }
            CloseAudioDevice();
            CloseWindow();
        }


        public static double MapRange(double s, double a1, double a2, double b1, double b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        public static float MapRange(float s, float a1, float a2, float b1, float b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);


        public static void DrawRectangleLinesV(Vector2 position, Vector2 size, Color color) =>
            DrawRectangleLines((int)position.X, (int)position.Y, (int)size.X, (int)size.Y, color);
        public static void DrawCircleLinesV(Vector2 position, float radius, Color color) =>
            DrawCircleLines((int)position.X, (int)position.Y, radius, color);
        public static void DrawTextV(string text, Vector2 position, int fontSize, Color color) =>
            DrawText(text, (int)position.X, (int)position.Y, fontSize, color);


        public static void AddSound(string soundName, string soundPath) =>
            Audio.Sounds.Add(soundName, LoadSound(Path.Combine(AssestsFolder, soundPath)));


        public static TEntity GetEntity<TEntity>() where TEntity : Entity => Entities.OfType<TEntity>().FirstOrDefault();

        public static IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : Entity => Entities.OfType<TEntity>();

        public static void RemoveEntitiesOfType<TEntity>() where TEntity : Entity
        {
            foreach (var e in Entities.OfType<TEntity>()) RemoveEntity(e);
        }

        public static void RemoveEntity(Entity e)
        {
            EventActions.Add(() =>
            {
                KeyBoard.EmitEvent -= e.OnKeyBoardEvent;
                Mouse.EmitEvent -= e.OnMouseEvent;
                Entities.Remove(e);
            });
        }

        public static void AddEntity(Entity e) => AddEntity(e, null, null);

        public static void AddEntity(Entity e, Action<IGuiEvent> onGuiEvent) => AddEntity(e, new[] { Audio.OnGuiEvent, onGuiEvent }, null);

        public static void AddEntity(Entity e, Action<IGameEvent> onGameEvent) => AddEntity(e, null, new[] { Audio.OnGameEvent, onGameEvent });



        public static void SetKeyBoardEventAction(Action<IKeyBoardEvent> action) => SetEmitEventActions(KeyBoard, action);

        public static void SetMouseEventAction(Action<IMouseEvent> action) => SetEmitEventActions(Mouse, action);


        #endregion


        #region internal api
        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, List<Action<T>> onEventActions) where T : IEvent
        {
            foreach (var action in onEventActions)
            {
                e.EmitEvent += action;

                //ignore mouse when loggin as it is way too spammy
                //same may apply to keyboard as well, we'll see
                if (DoEventLogging && e is not Mouse m)
                    e.EmitEvent += a =>
                        {
                            Console.WriteLine(
                                $"Frame    | {FrameCount}\n" +
                                $"Emitter  | {e.GetType()} {e.GetHashCode()}\n" +
                                $"Event    | {a.GetType()}\n" +
                                $"Receiver | {action.GetMethodInfo().DeclaringType}.{action.GetMethodInfo().Name} {action.GetHashCode()}" +
                                $"\n");
                        };

            }
        }

        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent => SetEmitEventActions(e, onEventActions?.ToList() ?? new());

        internal static void OnGuiEvent(IGuiEvent e)
        {
            if (e is IHasUndoRedo ur)
                UndoStack.Push(ur);

            if (e is DeleteEdit edit)
                RemoveEntity(edit.GuiEntity);
        }

        internal static void OnKeyBoardEvent(IKeyBoardEvent kbe)
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

        internal static void OnMouseEvent(IMouseEvent me)
        {

        }

        #endregion

        private static void AddEntity(Entity e,
           Action<IGuiEvent>[] onGuiEventActions = null,
           Action<IGameEvent>[] onGameEventActions = null)
        {
            EventActions.Add(() =>
            {
                EntityEventInitialisation(e, onGuiEventActions, onGameEventActions);
                Entities.Add(e);
            });
        }

        private static void EntityEventInitialisation(
           Entity e,
           Action<IGuiEvent>[] onGuiEventActions = null,
           Action<IGameEvent>[] onGameEventActions = null)
        {

            if (e is IKeyBoardListener kbl) SetEmitEventActions(KeyBoard, kbl.OnKeyBoardEvent);
            if (e is IMouseListener ml) SetEmitEventActions(Mouse, ml.OnMouseEvent);
            if (e is IEventEmitter<IGameEvent> pe) SetEmitEventActions(pe, onGameEventActions);
            if (e is IEventEmitter<IGuiEvent> eui) SetEmitEventActions(eui, onGuiEventActions);
            if (e is DragEditShape des) SetEmitEventActions(des, OnGuiEvent);
        }

        private static long GetFrameTime(ref long past)
        {
            var now = sw.ElapsedTicks;
            var delta = now - past;
            past = now;
            return delta;
        }

        private static void DoCollisions()
        {
            var colliders = Entities.OfType<IHasCollider>().ToArray();

            for (var i = 0; i < colliders.Length; i++)
            {
                var e1 = colliders[i];
                if (e1 is IHasCollision cl1)
                {
                    for (var j = 0; j < colliders.Length; j++)
                    {
                        var e2 = colliders[j];
                        if (e1 != e2 && e1.Collider.Overlaps(e2.Collider))
                            cl1.OnCollision(e2);
                    }
                }
            }
        }

        private static double FixedUpdateInterval = 1000 / FixedUpdate * TickMultiplier;
        private static double ElapsedUpdateInterval = 0d;
        private static void DoFixedUpdate(double frameTime)
        {
            ElapsedUpdateInterval += frameTime;
            //TODO handle frametime lower than fixed update, i.e. handle 2 update calls per frame
            if (ElapsedUpdateInterval >= FixedUpdateInterval)
            {
                foreach (var e in Entities) e.Update(ElapsedUpdateInterval);
                ElapsedUpdateInterval = 0d;
            }
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
    }
}
