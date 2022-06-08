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
global using SharpRay.Listeners;
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
            Load();

            //File.WriteAllLines(Path.Combine(AssestsFolder, "stats.txt"), Asteroid.GetStats());

            AddEntity(new StarField());
            AddEntity(Gui.CreateShipSelectionMenu());
            StartGame();
            Run();
        }

        public static LevelData testLevel => new(
            Description: "Test Level",
            WinScore: 100,
            ShipLayout: new(
                Position: new(WindowWidth / 2, WindowHeight / 2),
                Health: MaxHealth),
            Lifes: 3,
            AsteroidSpawnStart: new()
            {
                new (Asteroid.Size.Large, Asteroid.Type.Dirt, new (800, 100), new (0, 1.5f)),
            },
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
            },
            InitialHeadingSpeed: new Vector2(1.5f, 1.5f),
            MaxSpawnTime: 1500f * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 5000f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    Description = "Triple Shooter Weapon!",
                    SpawnScore = 15,
                    OnPickUp = s => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.TripleNarrow)
                },
                new()
                {
                    Description = "Bullets does 2x Damage!",
                    SpawnScore = 30,
                    OnPickUp = s => PrimaryWeapon.ChangeBulletType(Bullet.Type.Heavy)
                }

            });

        public static void StartGame()
        {
            PlayerLifes = testLevel.Lifes;
            PrimaryWeapon.OnStartGame();
            var level = new Level();
            level.OnEnter(testLevel);
            AddEntity(level);
        }

        private static void ResetGame()
        {
            StopAllSounds();

            //remove entities
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<Level>();
            RemoveEntitiesOfType<PickUp>();
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.Notification));

            //reset game stats
            Score = 0;
            PlayerLifes = MaxPlayerLifes;

            //generate new back ground
            GetEntity<StarField>().Generate();
        }

        internal static void OnGuiEvent(IGuiEvent e)
        {
            if (e is NextLevel nl)
            {
                GetEntity<Level>().OnEnter(testLevel);
            }
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                RemoveEntity(sha.Asteroid);
                PlaySound(Sounds[Ship.HitSound]);

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

                    //TODO better method name & reset the pickup spawns
                    PrimaryWeapon.OnStartGame();
                    var level = GetEntity<Level>();
                    level.PickUpScore = 0;
                    level.Data.PickUps.Where(p => p.HasSpawned && !GetEntities<PickUp>().Contains(p))
                        .ToList().ForEach(p => p.HasSpawned = false);

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
                    //StartGame();
                    GetEntityByTag<GuiContainer>(Gui.Tags.ShipSelection).Show();
                }
            }

            if (e is ShipPickUp spu)
            {
                IsPaused = true;
                PlaySound(Sounds[PickUp.PickupSound]);
                RemoveEntity(spu.PickUp);
            }

            if (e is AsteroidDestroyed ad)
            {
                //update gui
                var hp = Asteroid.GetHitPoints(ad.Asteroid.Definition);
                Score += hp;
                GetEntity<Level>().PickUpScore += hp;
                GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay).OnGameEvent(e);


                SetSoundPitch(Sounds[Asteroid.ExplosionSound], GetRandomValue(50, 150) / 100f);
                PlaySound(Sounds[Asteroid.ExplosionSound]);

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
