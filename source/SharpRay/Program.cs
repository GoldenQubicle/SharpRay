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
            new UIEntityContainer(new List<UIEntity>
            {
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(WindowWidth / 3, WindowHeight / 3 * 2),
                    Margins = new Vector2(20, 40),
                    FillColor = DARKBROWN,
                    TextColor = GOLD,
                    FontSize = 70f,
                    Text = "Game Over",
                },
                new Label
                {
                    Position = new Vector2(0, 200),
                    Size = new Vector2(WindowWidth / 3, WindowHeight / 6),
                    Margins = new Vector2(30, 35),
                    FillColor = BLANK,
                    TextColor = ORANGE,
                    FontSize = 45,
                    OnGameEventAction = (e, l) =>
                    {
                        if(e is SnakeGameOver go) (l as Label).Text = $"SCORE : {go.Score}";
                    }
                },
                new Button
                {
                    Position = new Vector2(150, 120),
                    Size = new Vector2(100, 100),
                    Margins = new Vector2(12,  3),
                    BaseColor = DARKBLUE,
                    FocusColor = BLUE,
                    TextColor = ORANGE,
                    OnMouseLeftClick = e => new SnakeGameStart { UIComponent = e },
                    Text = "AGAIN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
                    FontSize = 37,
                }

            }, new Vector2(WindowWidth / 2 - WindowWidth / 6, WindowHeight / 2 - WindowHeight / 3))
            {
                IsVisible = false,
                OnUIEventAction = (e, c) =>
                {
                    if (e is SnakeGameStart) (c as UIEntityContainer).Hide();
                }
            },
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
                    OnMouseLeftClick = e => new SnakeGameStart { UIComponent = e }
                }
            }, new Vector2(WindowWidth / 2 - 100, WindowHeight / 2 - 120))
            {
                IsVisible = true,
                OnUIEventAction = (e, c) =>
                {
                    if (e is SnakeGameStart) (c as UIEntityContainer).Hide();
                }
            },
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
            SetWindowPosition(1366, 712);
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

                if (e is UIEntityContainer c)
                {

                    foreach (var ce in c.Entities) SetEmitEventActions(ce, OnUIEvent, Audio.OnUIEvent, c.OnUIEvent);
                }
            }
        }

        private static void EntityEventInitialisation(params Entity[] entities) => EntityEventInitialisation(entities.ToList());

        private static void SetEmitEventActions<T>(IEventEmitter<T> e, params Action<T>[] onEventActions) where T : IEvent
        {
            foreach (var action in onEventActions) e.EmitEvent += action;
        }

        private static void SetBackGround() =>
            Entities.Insert(0, new ImageTexture(GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, GOLD, ORANGE), DARKBROWN));

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
            EventActions.ForEach(a => a());
            EventActions.Clear();
        }

        #endregion


        #region snak gam

        static Random rnd = new();

        private static void OnGameEvent(IGameEvent e)
        {

            if (e is SnakeConsumedFood f)
            {
                EventActions.Add(() =>
                {
                    Entities.Remove(f.FoodParticle);
                    var idx = Entities.Count - f.SnakeLength; // ensure snake segments render above particles & background
                    Entities.Insert(idx, f.NextSegment);
                    EntityEventInitialisation(f.NextSegment);
                });
            }

            if (e is SnakeConsumedPoop p)
            {
                //EventActions.Add(() =>
                //{
                //    Entities.Remove(p.PoopParticle);
                //    Entities.Remove(p.Tail);
                //});
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

            if (e is FoodParticleSpawn)
            {
                EventActions.Add(() =>
                {
                    var x = MapRange(rnd.NextDouble(), 0d, 1d, 0d, WindowWidth);
                    var y = MapRange(rnd.NextDouble(), 0d, 1d, 0d, WindowHeight);
                    var fp = new ParticleFood
                    {
                        Position = new Vector2((float)x, (float)y),
                        //Position = new Vector2(460, 200),
                        Size = new Vector2(CellSize, CellSize),
                    };
                    EntityEventInitialisation(fp);
                    Entities.Insert(4, fp); // ensure rendering above background, uix2 & particlespawner
                });
            }

            if(e is PoopParticleSpawn ps)
            {
                EventActions.Add(() =>
                {
                    var pp = new ParticlePoop
                    {
                        Position = ps.GameEntity.Position,
                        Size = new Vector2(CellSize, CellSize)
                    };
                    EntityEventInitialisation(pp);
                    Entities.Insert(4, pp); // ensure rendering above background, uix2 & particlespawner
                });
            }
        }

        private static void OnUIEvent(IUIEvent e)
        {
            if (e is SnakeGameStart)
            {
                var x = 360;
                var head = new Snake(new Vector2(x, 200))
                {
                    Bounds = new Vector2(WindowWidth, WindowHeight),
                    Direction = Direction.Right,
                    NextDirection = Direction.Right,
                };
                //var neck = head.SetNext();
                //var tail = neck.SetNext();
                //head.Segments.Add(neck);
                //head.Segments.Add(tail);

                //bind to pass game over event to score label ui
                head.EmitEvent += Entities.OfType<UIEntityContainer>().First().Entities.OfType<IGameEventListener>().ToArray()[1].OnGameEvent; // hot damn this is ugly

                var spawner = new ParticleSpawner { Size = new Vector2(WindowWidth, WindowHeight) };

                EntityEventInitialisation(head,  spawner);
                Entities.Add(spawner);
                Entities.Add(head);
                //Entities.Add(neck);
                //Entities.Add(tail);
            }
        }

        #endregion
    }
}
