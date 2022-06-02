global using System;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using Raylib_cs;
global using static Raylib_cs.Raylib;
global using SharpRay.Core;
global using SharpRay.Components;
global using SharpRay.Collision;
global using SharpRay.Eventing;
global using SharpRay.Entities;
global using SharpRay.Interfaces;
global using SharpRay.Gui;
global using static SharpRay.Core.Application;
global using static SharpRay.Core.Audio;
global using static Asteroids.GuiEvents;
global using static Asteroids.Assets;
global using static Asteroids.Game;

namespace Asteroids
{
    public static class Game
    {
        internal const int WindowWidth = 1080;
        internal const int WindowHeight = 720;

        //Render layers
        internal const int RlBackground = 0;
        internal const int RlGuiShipSelection = 1;
        internal const int RlAsteroidsBullets = 2;
        internal const int RlPickUps = 3;
        internal const int RlShip = 4;
        internal const int RlGuiScoreOverlay = 5;

        //Game state & stats
        internal static int ShipType = 3; // 1 | 2 | 3
        internal static string ShipColor = Gui.Green;
        internal static int ShipDamageTextureIdx = -1; // 1 | 2 | 3, initialized at -1 because reasons..
        internal static int Score = 0;
        //internal static int Health;
        internal static readonly int MaxHealth = 10;
        internal static int PlayerLifes;
        internal static readonly int MaxPlayerLifes = 3;

        public static bool IsPaused { get; set; }

        static async Task Main(string[] args)
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = new Color(12, 24, 64, 0),
                ShowFPS = true,
                DoEventLogging = true
            });

            SetKeyBoardEventAction(OnKeyBoardEvent);
            await Load();

            File.WriteAllLines(Path.Combine(AssestsFolder, "stats.txt"), Asteroid.GetStats());

            AddEntity(new StarField());
            //AddEntity(CreateShipSelectionMenu());
            StartGame();
            Run();
        }

        static LevelData testLevel => new(
            SpawnStart: new()
            {
                new Asteroid(Asteroid.Size.Large, Asteroid.Type.Dirt, new Vector2(800, 100), new Vector2(0, 1.5f)),
            },
            SpawnDuring: new()
            {
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Tiny, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
            },
            InitialHeadingSpeed: new Vector2(1.5f, 1.5f),
            SpawnTime: 20000f,
            Easing: Easings.EaseSineInOut,
            PickUps: new()
            {
                new PickUp
                {
                    Description = "Triple Shooter Weapon!",
                    OnPickUp = s => s.PrimaryWeapon = new WeaponTripleShooter()
                }
            });

        public static void StartGame()
        {
            PlayerLifes = MaxPlayerLifes;

            var ship = new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[ShipType][ShipColor]));
            var overlay = Gui.CreateScoreOverLay();
            var notice = Gui.CreateNotification();
            ship.EmitEvent += OnGameEvent;
            ship.EmitEvent += overlay.OnGameEvent;
            ship.EmitEvent += notice.OnGameEvent;
            overlay.Show();

            AddEntity(ship);
            AddEntity(overlay);
            AddEntity(notice);
            AddEntity(new Level(testLevel));
            IsPaused = false;
        }

        private static void ResetGame()
        {
            StopAllSounds();

            //remove entities
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<Level>();
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.Notification));

            //reset game stats
            Score = 0;
            PlayerLifes = MaxPlayerLifes;

            //generate new back ground
            GetEntity<StarField>().Generate();
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                RemoveEntity(sha.Asteroid);

                var idx = (int)MapRange(sha.ShipHealth, 0, MaxHealth, 3, 1);

                if (idx != ShipDamageTextureIdx && idx <= 3)
                {
                    ShipDamageTextureIdx = idx;
                    GetEntity<Ship>().DamgageTexture = GetTexture2D(shipDamage[ShipType][ShipDamageTextureIdx]);
                }

                //player life lost
                if (sha.LifeLost)
                {
                    IsPaused = true;

                    var ship = GetEntity<Ship>();
                    ship.HasTakenDamage = false; // prevent damage texture from being visible
                    ship.Health = MaxHealth;
                    ship.Position = new Vector2(WindowWidth / 2, WindowHeight / 2);

                    ShipDamageTextureIdx = -1;
                    PlayerLifes--; 

                  
                }

                if (PlayerLifes == 0)
                {
                    ResetGame();
                    StartGame();
                    //GetEntityByTag<GuiContainer>(GuiShipSelection).Show();
                }
            }

            if (e is ShipPickUp spu)
            {
                IsPaused = true;
                var notice = GetEntityByTag<GuiContainer>(Gui.Tags.Notification);
                notice.GetEntityByTag<Label>(Gui.Tags.Notification).Text = spu.PickUp.Description;
                notice.Show();
                RemoveEntity(spu.PickUp);
            }

            if (e is BulletLifeTimeExpired ble)
            {
                RemoveEntity(ble.Bullet);
            }

            if (e is AsteroidDestroyed ad)
            {
                //update gui
                Score += Asteroid.GetHitPoints(ad.Asteroid.Definition);
                GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay)
                    .GetEntityByTag<Label>(Gui.Tags.Score).Text = Gui.GetScoreString(Score);

                //spawn new asteroids from the one destroyed
                var spawns = Asteroid.GetSpawns(ad.Asteroid.Definition);
                foreach (var (s, i) in spawns.Select((s, i) => (s, i)))
                {
                    var angle = MathF.Tau / spawns.Count * i + (DEG2RAD * GetRandomValue(-10, 10));
                    var heading = ad.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    AddEntity(new Asteroid(s.Size, s.Type, ad.Asteroid.Position, heading), OnGameEvent);
                }

                // remove the entities 
                RemoveEntity(ad.Asteroid);
                RemoveEntity(ad.Bullet);
            }
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            
            if (e is KeyPressed kp)
            {
                if (kp.KeyboardKey == KeyboardKey.KEY_E)
                {
                    ResetGame();
                    StartGame();
                }

                if (kp.KeyboardKey == KeyboardKey.KEY_M)
                {
                    //TODO prompt to stop and exit to menu..
                    ResetGame();
                    GetEntityByTag<GuiContainer>(Gui.Tags.ShipSelection).Show();
                }
            }
        }
    }
}
