using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static SharpRay.SnakeConfig;
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
        public static List<Entity> Entities = new()
        {
            UIEntityContainerBuilder.CreateNew(isVisible: false).AddChildren(
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(320, WindowHeight),
                    Margins = new Vector2(20, 20),
                    FillColor = DARKBROWN,
                    TextColor = GOLD,
                    FontSize = 90f,
                    Text = "Game Over",
                },
                new Label
                {
                    Position = new Vector2(0, 300),
                    Size = new Vector2(320, WindowHeight / 6),
                    Margins = new Vector2(60, 20),
                    FillColor = DARKPURPLE,
                    TextColor = YELLOW,
                    FontSize = 45,
                },
                new Button
                {
                    Position = new Vector2(200, 160),
                    Size = new Vector2(80, 100),
                    Margins = new Vector2(10, 3),
                    BaseColor = DARKBLUE,
                    FocusColor = BLUE,
                    TextColor = ORANGE,
                    OnMouseLeftClick = e => new SnakeGameStart { UIComponent = e },
                    Text = "AGAIN!",
                    FontSize = 37,
                })
            .Translate(new Vector2(WindowWidth / 2 - 320 / 2, 0))
            .OnUIEvent((e, c) =>
                {
                    if (e is SnakeGameStart) c.Hide();
                })
            .OnGameEvent((e, c) =>
                {
                    if (e is SnakeGameOver go)
                        (c.Entities[1] as Label).Text = $"SCORE : {go.Score}";
                }),

            UIEntityContainerBuilder.CreateNew().AddChildren(
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(320, WindowHeight),
                    Margins = new Vector2(18, 40),
                    FillColor = DARKBROWN,
                    TextColor = GOLD,
                    FontSize = 110f,
                    Text = "Shitty Snake",
                },
                new Button
                {
                    Position = new Vector2(120, 160),
                    Size = new Vector2(120, 40),
                    Margins = new Vector2(14, 3),
                    BaseColor = DARKBLUE,
                    FocusColor = MAGENTA,
                    TextColor = ORANGE,
                    Text = "Start",
                    FontSize = 37,
                    OnMouseLeftClick = e => new SnakeGameStart { UIComponent = e }
                })
            .Translate(new Vector2(WindowWidth / 2 - 320 / 2, 0))
            .OnUIEvent((e, c) =>
            {
                if (e is SnakeGameStart) c.Hide();
            }),
        };

        public const string AssestsFolder = @"C:\Users\Erik\source\repos\SharpRayEngine\assests";
        public const double TickMultiplier = 10000d;

        private static readonly List<Action> EventActions = new();
        private static readonly Stopwatch sw = new();

        public static double MapRange(double s, double a1, double a2, double b1, double b2) => b1 + ((s - a1) * (b2 - b1)) / (a2 - a1);

        static void Main(string[] args)
        {
            EntityEventInitialisation(Entities);

            InitAudioDevice();
            Audio.Initialize();

            InitWindow(WindowWidth, WindowHeight, Assembly.GetEntryAssembly().GetName().Name);
            SetWindowPosition(GetMonitorWidth(0)/2-WindowWidth/2, GetMonitorHeight(0)/2-WindowHeight/2);
            SetBackGround();

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

        #region engine stuff

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

        private static void SetBackGround() =>
            Entities.Insert(0, new ImageTexture(GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, LIGHTGRAY, GRAY), DARKBLUE));

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

        #endregion


        #region snak gam
        private static void OnGameEvent(IGameEvent e)
        {

            if (e is SnakeConsumedFood f)
            {
                EventActions.Add(() =>
                {
                    Entities.Remove(f.FoodParticle);

                    EntityEventInitialisation(f.NextSegment);
                    var idx = Entities.Count - f.SnakeLength; // ensure snake segments render above particles & background
                    Entities.Insert(idx, f.NextSegment);
                });
            }

            if (e is DespawnPoop p)
            {
                EventActions.Add(() =>
                {
                    Entities.Remove(p.PoopParticle);
                });
            }

            if (e is SnakeGameOver go)
            {
                EventActions.Add(() =>
                {
                    var preceding = 3;//background, start & game over menu, don't want to remove those                   
                    Entities.RemoveRange(preceding, Entities.Count - preceding);
                    Entities.OfType<UIEntityContainer>().First().Show();
                });
            }

            if (e is FoodParticleSpawn fs)
            {
                EventActions.Add(() =>
                {
                    var fp = new ParticleFood
                    {
                        Position = fs.Position,
                        Size = new Vector2(FoodSize, FoodSize),
                    };
                    EntityEventInitialisation(fp);
                    Entities.Insert(4, fp); // ensure rendering above background, uix2 & particlespawner
                });
            }

            if (e is PoopParticleSpawn ps)
            {
                EventActions.Add(() =>
                {
                    var pp = new ParticlePoop
                    {
                        Position = ps.Position,
                        Size = new Vector2(PoopSize, PoopSize)
                    };
                    EntityEventInitialisation(pp);
                    Entities.Insert(4, pp); // ensure rendering above background, uix2 & particlespawner
                });
            }
        }

        public static void OnUIEvent(IUIEvent e)
        {
            if (e is SnakeGameStart)
            {
                var head = new Snake(new Vector2(400, 160))
                {
                    Bounds = new Vector2(WindowWidth, WindowHeight),
                    Direction = Direction.Right,
                    NextDirection = Direction.Right,
                };

                var spawner = new FoodParticleSpawner
                {
                    Size = new Vector2(WindowWidth, WindowHeight)
                };

                EntityEventInitialisation(head, spawner);

                spawner.Initialize(FoodParticleStart); //set 1st random interval and food particles to start with 

                //create 3 segment snake to start with bc 2 part snake doesn't collide with itself yet (due to locomotion)
                var neck = head.SetNext();
                var tail = neck.SetNext();
                head.Segments.Add(neck);
                head.Segments.Add(tail);

                //binding in order to get game over event to game over screen, and spawner tracks particles 
                head.EmitEvent += Entities.OfType<UIEntityContainer>().First().OnGameEvent;
                head.EmitEvent += spawner.OnGameEvent;

                //add it all to entities and note insertion order matters w regards to main loop. spawner first, head last!
                Entities.Add(spawner);
                Entities.Add(tail);
                Entities.Add(neck);
                Entities.Add(head);
            }
        }
        #endregion
    }
}
