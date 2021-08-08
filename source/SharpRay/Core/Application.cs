using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static SharpRay.Core.Config;
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
using System.Timers;

namespace SharpRay.Core
{
    public class Application
    {
        public static string AssestsFolder = Path.Combine(AppContext.BaseDirectory, @"assests");

        private static readonly Stack<IHasUndoRedo> UndoStack = new();
        private static readonly Stack<IHasUndoRedo> RedoStack = new();
        private static readonly List<Action> EventActions = new();
        private static readonly Stopwatch sw = new();
        private static List<Entity> Entities = new();

        public static void RemoveEntity(Entity e)
        {
            EventActions.Add(() =>
            {
                KeyBoard.EmitEvent -= e.OnKeyBoardEvent;
                Mouse.EmitEvent -= e.OnMouseEvent;
                Entities.Remove(e);
            });
        }

        public static void AddEntity(Entity e)
        {
            EntityEventInitialisation(e);
            Entities.Add(e);
        }

        public static void AddEntity(Entity e, Action<IGuiEvent> onGuiEvent)
        {
            EntityEventInitialisation(e, Audio.OnUIEvent, onGuiEvent);
            Entities.Add(e);
        }

        public static void AddEntity(Entity e, Action<IGameEvent> onGameEvent)
        {
            EntityEventInitialisation(e, Audio.OnGameEvent, onGameEvent);
            Entities.Add(e);
        }

        public static double MapRange(double s, double a1, double a2, double b1, double b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        public static float MapRange(float s, float a1, float a2, float b1, float b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);

        public static void Run(Config config)
        {
            Mouse.EmitEvent += OnMouseEvent;
            KeyBoard.EmitEvent += OnKeyBoardEvent;

            InitAudioDevice();
            Audio.Initialize();
            InitWindow(config.WindowWidth, config.WindowHeight, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(GetMonitorWidth(0) / 2 + 128, GetMonitorHeight(0) / 2 - config.WindowHeight / 2);

            //SetTargetFPS(60);

            sw.Start();
            var previous = 0L;

            while (!WindowShouldClose())
            {
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


        private static void EntityEventInitialisation(
           Entity e,
           Action<IGuiEvent>[] onGuiEventActions = null,
           Action<IGameEvent>[] onGameEventActions = null)
        {

            if (e is IKeyBoardListener kbl) KeyBoard.EmitEvent += kbl.OnKeyBoardEvent;
            if (e is IMouseListener ml) Mouse.EmitEvent += ml.OnMouseEvent;
            if (e is IEventEmitter<IGameEvent> pe) SetEmitEventActions(pe, onGameEventActions);
            if (e is IEventEmitter<IGuiEvent> eui) SetEmitEventActions(eui, onGuiEventActions);

        }

        private static void EntityEventInitialisation(
            Entity[] entities,
            Action<IGuiEvent>[] onGuiEventActions = null,
            Action<IGameEvent>[] onGameEventActions = null)
        {
            foreach (var e in entities) EntityEventInitialisation(e, onGuiEventActions, onGameEventActions);
        }

        private static void EntityEventInitialisation(Entity e, Action<IGuiEvent> onGuiEvent) => EntityEventInitialisation(e, new[] { onGuiEvent }, null);
        private static void EntityEventInitialisation(Entity e, params Action<IGuiEvent>[] onGuiEvents) => EntityEventInitialisation(e, onGuiEvents, null);
        private static void EntityEventInitialisation(Entity e, Action<IGameEvent> onGameEvent) => EntityEventInitialisation(e, null, new[] { onGameEvent });
        private static void EntityEventInitialisation(Entity e, params Action<IGameEvent>[] onGameEvents) => EntityEventInitialisation(e, null, onGameEvents);

        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, List<Action<T>> onEventActions) where T : IEvent
        {
            foreach (var action in onEventActions) e.EmitEvent += action;
        }

        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent => SetEmitEventActions(e, onEventActions?.ToList() ?? new());

        private static long GetFrameTime(ref long past)
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

        private static bool CheckCollisionRecs(ICollider collider1, ICollider collider2) => collider1.Overlaps(collider2);


        private static double FixedUpdateInterval = 1000 / 60 * TickMultiplier;
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

        internal static void OnGameEvent(IGameEvent e)
        {

        }

        internal static void OnUIEvent(IGuiEvent e)
        {
            if (e is IHasUndoRedo ur)
                UndoStack.Push(ur);

            if (e is DeleteEdit edit)
                RemoveEntity(edit.GuiComponent);
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
    }
}
