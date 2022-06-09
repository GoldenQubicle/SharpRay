global using static Raylib_cs.Raylib;
global using static Raylib_cs.Color;
global using static SharpRay.Core.SharpRayConfig;
global using System.Reflection;
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Linq;
global using System.IO;
global using SharpRay.Collision;
global using SharpRay.Entities;
global using SharpRay.Eventing;
global using SharpRay.Gui;
global using SharpRay.Listeners;
global using System.Numerics;
global using Raylib_cs;
global using SharpRay.Interfaces;

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
        private static Dictionary<string, Texture2D> Textures = new();
        private static Dictionary<string, Font> Fonts = new();
        private static SortedDictionary<int, List<IHasRender>> RenderLayers = new();
        public static string AssestsFolder = Path.Combine(AppContext.BaseDirectory, @"assets");
        private static long FrameCount;
        private static bool DoEventLogging;
        private static bool ShowFPS;
        private static Color backGroundColor;

        #region public api

        /// <summary>
        /// Initialize the SharpRay application with the given configuration.
        /// <see cref="SharpRayConfig"/> for available options.
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(SharpRayConfig config)
        {
            ShowFPS = config.ShowFPS;
            DoEventLogging = config.DoEventLogging;
            backGroundColor = config.BackGroundColor;

            Mouse.EmitEvent += OnMouseEvent;
            KeyBoard.EmitEvent += OnKeyBoardEvent;

            InitAudioDevice();
            Audio.Initialize();
            InitWindow(config.WindowWidth, config.WindowHeight, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(GetMonitorWidth(0) / 2 - config.WindowWidth / 2, GetMonitorHeight(0) / 2 - config.WindowHeight / 2);

            //SetTargetFPS(60);
        }

        /// <summary>
        /// The main loop of the SharpRay application. 
        /// Handles rendering, collision, eventing, etc.
        /// Unloads assets from memory when the SharpRay application is closed. 
        /// </summary>
        public static void Run()
        {
            sw.Start();
            var previous = 0L;

            while (!WindowShouldClose())
            {
                if (DoEventLogging) FrameCount++;
                if (ShowFPS) DrawFPS(0, 0);

                var frameTime = GetFrameTime(ref previous);

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                DoCollisions();
                DoFixedUpdate(frameTime);
                DoRender();

                if (EventActions.Count > 0)
                    DoEventActions();
            }

            foreach (var t in Textures) UnloadTexture(t.Value);
            foreach (var a in Audio.Sounds) UnloadSound(a.Value);
            foreach (var f in Fonts) UnloadFont(f.Value);
            CloseAudioDevice();
            CloseWindow();
        }

        /// <summary>
        /// Convenient method to take advantage of .NET hot reloading while designing GUI elements. 
        /// Contineously adds & removes entities, DOES NOT support interactivity!
        /// </summary>
        /// <param name="action"></param>
        public static void RunDebugGui(Action action)
        {
            while (!WindowShouldClose())
            {
                action();
                DoEventActions();
                DoRender();
                Entities.Clear();
                RenderLayers.Clear();
            }
            CloseAudioDevice();
            CloseWindow();
        }


        /// <summary>
        /// Retrieves a <see cref="Font"/> from the Fonts dictionary. Will throw an exception if the given key is not present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="Font"/></returns>
        public static Font GetFont(string key) => Fonts[key];


        /// <summary>
        /// Loads a <see cref="Font"/> from file, and adds it the Fonts dictionary with the given key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fontFileName"></param>
        public static void AddFont(string key, string fontFileName) =>
            Fonts.Add(key, LoadFont(Path.Combine(AssestsFolder, fontFileName)));


        /// <summary>
        /// Loads a <see cref="Sound"/> from file, and adds it the Sounds dictionary with the given key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="soundFileName"></param>
        public static void AddSound(string key, string soundFileName) =>
            Audio.Sounds.Add(key, LoadSound(Path.Combine(AssestsFolder, soundFileName)));

        /// <summary>
        /// Plays the sound with the <see cref="Sound"/> from the Audio.Sounds dictionary with the given key.
        /// </summary>
        /// <param name="key"></param>
        public static void PlaySound(string key) => Raylib.PlaySound(Audio.Sounds[key]);


        /// <summary>
        /// Retrieves a <see cref="Texture2D"/> from the Textures dictionary. Will throw an exception if the given key is not present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="Texture2D"/></returns>
        public static Texture2D GetTexture2D(string key) => Textures[key];


        /// <summary>
        /// Loads a <see cref="Texture2D"/> from file, and adds it to the Textures dictionary with the given key.
        /// </summary>
        /// <param name="key">The key used in texture dictionary</param>
        /// <param name="filePath">relative to asset folder</param>
        public static void AddTexture2D(string key, string filePath) => Textures.Add(key, LoadTexture(Path.Combine(AssestsFolder, filePath)));


        /// <summary>
        /// Gets the first <typeparamref name="TEntity"/> from the Entity list.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static TEntity GetEntity<TEntity>() where TEntity : Entity => Entities.OfType<TEntity>().FirstOrDefault();

        /// <summary>
        /// Gets an entity by tag, assumes every entity has an unique tag!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TEntity GetEntityByTag<TEntity>(string tag) where TEntity : Entity =>
            Entities.OfType<TEntity>().FirstOrDefault(e => e.Tag.Equals(tag));


        /// <summary>
        /// Gets all <typeparamref name="TEntity"/> from the Entity list. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : Entity => Entities.OfType<TEntity>();


        /// <summary>
        /// Removes all <typeparamref name="TEntity"/> from the Entity list, and unsubscribes them from events. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public static void RemoveEntitiesOfType<TEntity>() where TEntity : Entity
        {
            foreach (var e in Entities.OfType<TEntity>()) RemoveEntity(e);
        }


        /// <summary>
        /// Remove the given Enity from the Enity list,and unsubscribe from events. 
        /// </summary>
        /// <param name="e"></param>
        public static void RemoveEntity(Entity e)
        {
            EventActions.Add(() =>
            {
                KeyBoard.EmitEvent -= e.OnKeyBoardEvent;
                Mouse.EmitEvent -= e.OnMouseEvent;
                Entities.Remove(e);

                if (e is IHasRender)
                    RenderLayers[e.RenderLayer].Remove(e);
            });
        }


        /// <summary>
        /// Add an Entity to the Entity list. Binds it to KeyBoard and Mouse events by default. 
        /// </summary>
        /// <param name="e"></param>
        public static void AddEntity(Entity e) => AddEntity(e, null, null);


        /// <summary>
        /// Add an Entity to the Entity list. Binds it to KeyBoard and Mouse events by default. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="onGuiEvent">The method to call when the entity emits an IGuiEvent</param>
        public static void AddEntity(Entity e, Action<IGuiEvent> onGuiEvent) => AddEntity(e, new[] { Audio.OnGuiEvent, onGuiEvent }, null);


        /// <summary>
        /// /// Add an Entity to the Entity list. Binds it to KeyBoard and Mouse events by default. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="onGameEvent">The method to call when the entity emits an IGameEvent</param>
        public static void AddEntity(Entity e, Action<IGameEvent> onGameEvent) => AddEntity(e, null, new[] { Audio.OnGameEvent, onGameEvent });


        /// <summary>
        /// Binds KeyBoard events to the given method. Note GameEntities receive KeyBoard events by default. 
        /// </summary>
        /// <param name="action">The method to call when an KeyBoard event is emitted</param>
        public static void SetKeyBoardEventAction(Action<IKeyBoardEvent> action) => SetEmitEventActions(KeyBoard, action);


        /// <summary>
        /// Binds Mouse events to the given method. Note GameEntities receive Mouse events by default. 
        /// </summary>
        /// <param name="action">The method to call when a Mouse event is emitted</param>
        public static void SetMouseEventAction(Action<IMouseEvent> action) => SetEmitEventActions(Mouse, action);


        /// <summary>
        /// Maps a given source value to a target range value.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceMin"></param>
        /// <param name="sourceMax"></param>
        /// <param name="targetMin"></param>
        /// <param name="targetMax"></param>
        /// <returns></returns>
        public static double MapRange(double source, double sourceMin, double sourceMax, double targetMin, double targetMax) =>
            targetMin + (source - sourceMin) * (targetMax - targetMin) / (sourceMax - sourceMin);


        /// <summary>
        /// Maps a given source value to a target range value.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceMin"></param>
        /// <param name="sourceMax"></param>
        /// <param name="targetMin"></param>
        /// <param name="targetMax"></param>
        /// <returns></returns>
        public static float MapRange(float source, float sourceMin, float sourceMax, float targetMin, float targetMax) =>
            targetMin + (source - sourceMin) * (targetMax - targetMin) / (sourceMax - sourceMin);

        public static void DrawRectangleLinesV(Vector2 position, Vector2 size, Color color) =>
            DrawRectangleLines((int)position.X, (int)position.Y, (int)size.X, (int)size.Y, color);
        public static void DrawCircleLinesV(Vector2 position, float radius, Color color) =>
            DrawCircleLines((int)position.X, (int)position.Y, radius, color);
        public static void DrawTextV(string text, Vector2 position, int fontSize, Color color) =>
            DrawText(text, (int)position.X, (int)position.Y, fontSize, color);

        

        #endregion


        #region internal api
        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, List<Action<T>> onEventActions) where T : IEvent
        {
            foreach (var action in onEventActions)
            {
                e.EmitEvent += action;
            }

            //ignore mouse & keyboard when logging as it is way too spammy
            if (DoEventLogging && e is not Mouse m && e is not KeyBoard k)
                e.EmitEvent?.GetInvocationList().ToList().ForEach(d =>
                {
                    e.EmitEvent += a =>
                    {
                        Console.WriteLine(
                            $"Frame    | {FrameCount}\n" +
                            $"Emitter  | {e?.GetType()} {e?.GetHashCode()}\n" +
                            $"Event    | {a?.GetType()}\n" +
                            $"Receiver | {d.GetMethodInfo().DeclaringType}.{d?.GetMethodInfo().Name} {d?.GetHashCode()}" +
                            $"\n");
                    };
                });
        }

        internal static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent =>
            SetEmitEventActions(e, onEventActions?.ToList() ?? new());

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
                if (e is IHasRender)
                {
                    if (RenderLayers.ContainsKey(e.RenderLayer)) RenderLayers[e.RenderLayer].Add(e);
                    else RenderLayers.Add(e.RenderLayer, new List<IHasRender> { e });
                }
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
            ClearBackground(backGroundColor);
            foreach (var rl in RenderLayers.Values)
                foreach (var e in rl) e.Render();
            EndDrawing();
        }

        private static void DoEventActions()
        {
            foreach (var a in EventActions) a();
            EventActions.Clear();
        }
    }
}
