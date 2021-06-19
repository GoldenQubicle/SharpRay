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
            new UIEntityContainer(new List<UIEntity>
            {
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(200, 250),
                    Margins = new Vector2(25, 10),
                    FillColor = DARKBROWN,
                    TextColor = GOLD,
                    FontSize = 60f,
                    Text = "Shitty Snake",
                },
                new Button
                {
                    Position = new Vector2(25, 80),
                    Size = new Vector2(150, 20),
                    Margins = new Vector2(50, 3),
                    BaseColor = DARKBLUE,
                    FocusColor = BLUE,
                    TextColor = ORANGE,
                    Text = "Start",
                    OnMouseLeftClick = e => new StartSnakeGame { UIComponent = e }

                }
            }, new Vector2(Width / 2 - 100, Height / 2 - 120)),

        };
        private static GameEntity[] gameEntities;
        public const string AssestsFolder = @"C:\Users\Erik\source\repos\SharpRayEngine\assests";
        public const double TickMultiplier = 10000d;

        private static readonly List<Action> EventActions = new();
        private static readonly Stopwatch sw = new();
        static void Main(string[] args)
        {
            Mouse.EmitEvent += OnMouseEvent;
            KeyBoard.EmitEvent += OnKeyBoardEvent;

            EntityEventInitialisation(Entities);

            gameEntities = Entities.OfType<GameEntity>().ToArray();

            InitAudioDevice();
            Audio.Initialize();
            //SetTargetFPS(60);
            InitWindow(Width, Height, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(1366, 712);
            SetBackGround();

            sw.Start();
            var past = 0L;

            while (!WindowShouldClose())
            {
                var delta = GetDeltaTime(ref past);

                Mouse.DoEvents();
                KeyBoard.DoEvents();
                DoCollisions(gameEntities);
                DoRender(delta);
                DoEventActions();
            }

            CloseAudioDevice();
            CloseWindow();
        }

        private static void SetBackGround() =>
            Entities.Insert(0, new ImageTexture(GenImageChecked(Width, Height, 20, 20, GOLD, ORANGE), DARKBROWN));

        private static void EntityEventInitialisation(params Entity[] entities) => EntityEventInitialisation(entities.ToList());
        private static void EntityEventInitialisation(List<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e is IKeyBoardListener kbl) KeyBoard.EmitEvent += kbl.OnKeyBoardEvent;
                if (e is IMouseListener ml) Mouse.EmitEvent += ml.OnMouseEvent;

                if (e is IEventEmitter<IGameEvent> pe) SetEmitEventActions(pe, OnGameEvent, Audio.OnGameEvent);

                if (e is UIEntityContainer c) foreach (var ce in c.Entities) SetEmitEventActions(ce, OnUIEvent, Audio.OnUIEvent);
            }
        }

        private static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent
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

        private static void DoEventActions()
        {
            EventActions.ForEach(a => a());
            EventActions.Clear();
        }

        private static void DoRender(double delta)
        {
            BeginDrawing();
            ClearBackground(GRAY);

            foreach (var e in Entities) e.Render(delta);

            EndDrawing();
        }

        #region snak gam

        static Random rnd = new();
        private static void OnGameEvent(IGameEvent e)
        {
            if (e is SnakeConsumedFood f)
            {
                EventActions.Add(() =>
                {
                    Entities.Remove(f.FoodParticle);
                    EntityEventInitialisation(f.NextSegment);
                    var idx = Entities.Count - f.SnakeLength; // ensure snake segments render above particles
                    Entities.Insert(idx, f.NextSegment);
                    gameEntities = Entities.OfType<GameEntity>().ToArray();
                });
            }

            if (e is SnakeConsumedPoop p)
            {
                EventActions.Add(() =>
                {
                    Entities.Remove(p.GameEntity);
                    gameEntities = Entities.OfType<GameEntity>().ToArray();
                });
                // despawn segment, descrease score
            }

            if (e is SnakeCollideWithBody || e is SnakeCollideWithBounds)
            {
                // game over, update highscore, menu screen
            }

            if (e is ParticleSpawn)
            {
                EventActions.Add(() =>
                {
                    var x = MapRange(rnd.NextDouble(), 0d, 1d, 0d, Width);
                    var y = MapRange(rnd.NextDouble(), 0d, 1d, 0d, Height);
                    var fp = new FoodParticle
                    {
                        Position = new Vector2((float)x, (float)y),
                        //Position = new Vector2(460, 200),
                        Size = new Vector2(20, 20),
                    };
                    EntityEventInitialisation(fp);
                    Entities.Insert(3, fp);
                    gameEntities = Entities.OfType<GameEntity>().ToArray();
                });
            }
        }

        private static void OnUIEvent(IUIEvent e)
        {
            if (e is StartSnakeGame)
            {
                Entities.OfType<UIEntityContainer>().First().Hide();

                var head = new Head
                {
                    Position = new Vector2(380, 200),
                    Size = new Vector2(20, 20),
                    Bounds = new Vector2(Width, Height),
                    Direction = Direction.Right,
                    NextDirection = Direction.Right,
                };

                var spawner = new ParticleSpawner
                {
                    Size = new Vector2(Width, Height)
                };
                EntityEventInitialisation(head, spawner);
                Entities.Add(spawner);
                Entities.Add(head);
                gameEntities = Entities.OfType<GameEntity>().ToArray();
            }
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent kbe)
        {
            if (kbe is KeyPressed p && p.Char == 'M') Entities.OfType<UIEntityContainer>().First().Show();
        }

        private static void OnMouseEvent(IMouseEvent me)
        {

        }

        #endregion


    }
}
