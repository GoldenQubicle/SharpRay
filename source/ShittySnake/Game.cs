using System.Numerics;
using SharpRay.Core;
using SharpRay.Gui;
using SharpRay.Eventing;
using SharpRay.Entities;
using SnakeEvents;
using SnakeEntities;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static ShittySnake.Settings;

namespace ShittySnake
{
    public static class Game
    {
        static void Main(string[] args)
        {
            Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight, DoEventLogging = false });

            AddSound(nameof(SnakeGameStart), ButtonPushSound);
            AddSound(nameof(SnakeLocomotion), FootStepSound);
            AddSound(nameof(SnakeConsumedFood), SnakeGrow);
            AddSound(nameof(DespawnPoop), PoopDespawn);
            AddSound(nameof(FoodParticleSpawn), FoodSpawn);
            AddSound(nameof(PoopParticleSpawn), SnakeShrink);
            AddSound(nameof(SnakeGameOver), GameOver);

            CreateGui();

            Run();
        }

        /// <summary>
        /// create backdrop, 'again' & 'start' menus - in that order. Hides the former, shows the latter
        /// wires buttons to start the game
        /// </summary>
        private static void CreateGui()
        {
            AddEntity(new ImageTexture(GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, LIGHTGRAY, GRAY), DARKBLUE));
            AddEntity(GuiContainerBuilder.CreateNew(isVisible: false, "Menu").AddChildren(
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
                    OnMouseLeftClick = e => new SnakeGameStart { GuiEntity = e },
                    Text = "AGAIN!",
                    FontSize = 37,
                })
            .Translate(new Vector2(WindowWidth / 2 - 320 / 2, 0))
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
                {
                    (c.GetEntityByIndex(1) as Label).Text = $"SCORE : {go.Score}";
                    c.Show();
                }
            }));

            AddEntity(GuiContainerBuilder.CreateNew(isVisible: true, "Menu").AddChildren(
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
                    OnMouseLeftClick = e => new SnakeGameStart { GuiEntity = e }
                })
            .Translate(new Vector2(WindowWidth / 2 - 320 / 2, 0))
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
            InitializeFoodSpawner();
            InitializeHead();
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is SnakeConsumedFood f)
            {
                RemoveEntity(f.FoodParticle);
                AddEntity(f.NextSegment, OnGameEvent);
                GetEntity<FoodParticleSpawner>().OnGameEvent(e);
            }

            if (e is SnakeGameOver go)
                RemoveEntitiesOfType<GameEntity>();

            if (e is FoodParticleSpawn fs)
                AddEntity(new ParticleFood(fs.Position, FoodSize) { RenderLayer = "FoodAndPoop" });

            if (e is PoopParticleSpawn ps)
                AddEntity(new ParticlePoop(ps.Position, PoopSize) { RenderLayer = "FoodAndPoop" }, OnGameEvent);

            if (e is DespawnPoop p)
                RemoveEntity(p.PoopParticle);
        }

        private static void InitializeHead()
        {
            var head = new Snake(new Vector2(400, 160))
            {
                Bounds = new Vector2(WindowWidth, WindowHeight),
                Direction = Direction.Right,
                NextDirection = Direction.Right,
                RenderLayer = nameof(Snake)
            };

            AddEntity(head, OnGameEvent);
            AddEntity(head.SetNext(), OnGameEvent);
            AddEntity(head.Next.SetNext(), OnGameEvent);

            //binding to emit gameover event to the corresponding menu, which happens to be the 1st gui container added
            head.EmitEvent += GetEntity<GuiContainer>().OnGameEvent;
        }

        private static void InitializeFoodSpawner()
        {
            var spawner = new FoodParticleSpawner
            {
                Size = new Vector2(WindowWidth, WindowHeight)
            };

            AddEntity(spawner);
            spawner.EmitEvent += OnGameEvent; // binding since AddEntity executes as an EventAction at the end of drawing loop thus EmitEvent, called in spawner constructor, will be null when initialising.
            spawner.Initialize(FoodParticleStart); //set 1st random interval and food particles to start with 
        }
    }
}
