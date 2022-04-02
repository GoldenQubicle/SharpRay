using static SharpRay.Core.Application;
using SharpRay.Core;
using SharpRay.Gui;
using System.Numerics;
using static Raylib_cs.Color;
using SnakeEvents;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using SnakeEntities;
using System;
using SharpRay.Entities;
using System.Linq;

namespace ShittySnake
{
    public static class Game
    {
        static void Main(string[] args)
        {
            Initialize(new SharpRayConfig { WindowWidth = Settings.WindowWidth, WindowHeight = Settings.WindowHeight, DoEventLogging = true });
            CreateGui();

            Run();
        }

        /// <summary>
        /// create backdrop, 'again' & 'start' menus - in that order. Hides the former, shows the latter
        /// </summary>
        private static void CreateGui()
        {
            AddEntity(new ImageTexture(GenImageChecked(Settings.WindowWidth, Settings.WindowHeight, Settings.CellSize, Settings.CellSize, LIGHTGRAY, GRAY), DARKBLUE));
            AddEntity(GuiContainerBuilder.CreateNew(isVisible: false).AddChildren(
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(320, Settings.WindowHeight),
                    Margins = new Vector2(20, 20),
                    FillColor = DARKBROWN,
                    TextColor = GOLD,
                    FontSize = 90f,
                    Text = "Game Over",
                },
                new Label
                {
                    Position = new Vector2(0, 300),
                    Size = new Vector2(320, Settings.WindowHeight / 6),
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
                    OnMouseLeftClick = e => new SnakeGameStart { GuiEntity = e },
                    Text = "AGAIN!",
                    FontSize = 37,
                })
            .Translate(new Vector2(Settings.WindowWidth / 2 - 320 / 2, 0))
            .OnGuiEvent((e, c) =>
            {
                if (e is SnakeGameStart)
                {
                    c.Hide();
                    StartGame(e);
                }
            })
            .OnGameEvent((e, c) =>
            {
                if (e is SnakeGameOver go)
                    (c.GetEntityByIndex(1) as Label).Text = $"SCORE : {go.Score}";
            }));

            AddEntity(GuiContainerBuilder.CreateNew().AddChildren(
                new Label
                {
                    Position = new Vector2(),
                    Size = new Vector2(320, Settings.WindowHeight),
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
                    OnMouseLeftClick = e => new SnakeGameStart { GuiEntity = e }
                })
            .Translate(new Vector2(Settings.WindowWidth / 2 - 320 / 2, 0))
            .OnGuiEvent((e, c) =>
            {
                if (e is SnakeGameStart)
                {
                    c.Hide();
                    StartGame(e);
                }
            }));
        }

        private static void StartGame(IGuiEvent e)
        {
            var head = new Snake(new Vector2(400, 160))
            {
                Bounds = new Vector2(Settings.WindowWidth, Settings.WindowHeight),
                Direction = Direction.Right,
                NextDirection = Direction.Right,
            };

            var spawner = new FoodParticleSpawner
            {
                Size = new Vector2(Settings.WindowWidth, Settings.WindowHeight)
            };

            //EntityEventInitialisation(head, spawner);
            AddEntity(spawner);
            spawner.EmitEvent += OnGameEvent;
            spawner.Initialize(Settings.FoodParticleStart); //set 1st random interval and food particles to start with 

            //create 3 segment snake to start with bc 2 part snake doesn't collide with itself yet (due to locomotion)
            var neck = head.SetNext();
            var tail = neck.SetNext();
            head.Segments.Add(neck);
            head.Segments.Add(tail);

            //binding in order to get game over event to game over screen, and spawner tracks particles 
            head.EmitEvent += GetEntity<GuiContainer>().OnGameEvent;
            head.EmitEvent += spawner.OnGameEvent;

            //add it all to entities and note insertion order matters w regards to main loop. spawner first, head last!

            AddEntity(tail);
            AddEntity(neck);
            AddEntity(head, OnGameEvent);
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is SnakeLocomotion lm)
            {
                //Console.WriteLine("finally");
            }

            //if (e is SnakeConsumedFood f)
            //{
            //    EventActions.Add(() =>
            //    {
            //        Entities.Remove(f.FoodParticle);

            //        EntityEventInitialisation(f.NextSegment);
            //        var idx = Entities.Count - f.SnakeLength; // ensure snake segments render above particles & background
            //        Entities.Insert(idx, f.NextSegment);
            //    });
            //}

            //if (e is DespawnPoop p)
            //{
            //    EventActions.Add(() =>
            //    {
            //        Entities.Remove(p.PoopParticle);
            //    });
            //}

            if (e is SnakeGameOver go)
            {
                RemoveEntitiesOfType<GameEntity>();

                //EventActions.Add(() =>
                //{
                //    var preceding = 3;//background, start & game over menu, don't want to remove those                   
                //    Entities.RemoveRange(preceding, Entities.Count - preceding);
                //    //Entities.OfType<UIEntityContainer>().First().Show();
                //});

                GetEntities<GuiContainer>().First().Show();
            }

            if (e is FoodParticleSpawn fs)
            {
                AddEntity(new ParticleFood(fs.Position, Settings.FoodSize));

                //EventActions.Add(() =>
                //{
                //    var fp = new ParticleFood
                //    {
                //        Position = fs.Position,
                //        Size = new Vector2(FoodSize, FoodSize),
                //    };
                //    EntityEventInitialisation(fp);
                //    Entities.Insert(4, fp); // ensure rendering above background, uix2 & particlespawner
                //});
            }

            //if (e is PoopParticleSpawn ps)
            //{
            //    EventActions.Add(() =>
            //    {
            //        var pp = new ParticlePoop
            //        {
            //            Position = ps.Position,
            //            Size = new Vector2(PoopSize, PoopSize)
            //        };
            //        EntityEventInitialisation(pp);
            //        Entities.Insert(4, pp); // ensure rendering above background, uix2 & particlespawner
            //    });
            //}
        }
    }
}
