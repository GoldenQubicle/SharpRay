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
        internal static int CurrentScore = 0;
        internal static int CurrentHealth;
        internal static int MaxHealth = 10;
        internal static int CurrentLifes;
        internal static readonly int MaxLifes = 3;
        internal static readonly Color BackGroundColor = new(12, 24, 64, 0);
        internal static bool IsPaused { get; set; }
        internal static int LevelIdx = 0;
        //sound keys
        internal const string LifeLostSound1 = nameof(LifeLostSound1);
        internal const string LifeLostSound2 = nameof(LifeLostSound2);
        internal const string StartSound = nameof(StartSound);
        internal const string WinOverallSound = nameof(WinOverallSound);

        static async Task Main(string[] args)
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = BackGroundColor,
                ShowFPS = true,
                DoEventLogging = true
            });

            SetKeyBoardEventAction(OnKeyBoardEvent);
            Load();

            //RunDebugGui(() => AddEntity(Gui.CreateMainMenu()));
            AddEntity(new StarField());
            var mainmenu = Gui.CreateMainMenu();
            AddEntity(mainmenu);
            AddEntity(Gui.CreateShipSelectionMenu());

#if DEBUG
            StartGame(3);
#endif

#if RELEASE
            mainmenu.Show();
            PlaySound(Gui.SelectionSound, isRepeated: true);
#endif
            Run();
        }

        public static void StartGame(int lvlIdx)
        {
            HideCursor();
            LevelIdx = lvlIdx;
            CurrentHealth = MaxHealth;
            CurrentLifes = MaxLifes;
            PrimaryWeapon.OnGameStart();
            var level = new Level();
            level.OnEnter(Levels.Data[LevelIdx]);
            AddEntity(level);
        }

        public static void ResetGame()
        {
            //StopAllSounds();

            //remove entities
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<Level>();
            RemoveEntitiesOfType<PickUp>();

            //reset game stats
            CurrentScore = 0;
            CurrentLifes = MaxLifes;
            MaxHealth = 10;
            CurrentHealth = MaxHealth;

            //generate new back ground
            GetEntity<StarField>().Generate();
        }

        internal static void OnGuiEvent(IGuiEvent e)
        {
            if (e is NextLevel nl)
            {
                if (LevelIdx < Levels.Data.Count - 1)
                {
                    GetEntity<Level>().OnEnter(Levels.Data[++LevelIdx]);
                    return;
                }
                
                ShowCursor();
                GetEntityByTag<GuiContainer>(Gui.Tags.MainMenu).Show();
                RemoveEntitiesOfType<Level>();
                ResetGame();
                PlaySound(Gui.SelectionSound, true);
            }
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                RemoveEntity(sha.Asteroid);
                PlaySound(Ship.HitSound);
                UpdateShipDamageTexture(CurrentHealth);
            }

            if (e is ShipLifeLost sll)
            {
                //game state stuff
                IsPaused = true;
                CurrentLifes--;
                Gui.UpdateHealthOverlay(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay), MaxHealth);
                AddEntity(Gui.CreateShipLostNotification());
                PlaySound(LifeLostSound2);
                StopSound(Sounds[Ship.EngineSound]);
                StopSound(Sounds[Ship.ThrusterSound]);
                ShowCursor();

                //reset weapons & pickups 
                GetEntity<Level>().Data
                    .PickUps.Except(GetEntities<PickUp>()).ToList()
                    .ForEach(pu => pu.Reset());

                PrimaryWeapon.OnLifeLost();

                //reset ship position, health & texture
                GetEntity<Ship>().Reset();
                ShipDamageTextureIdx = -1;
            }

            if (e is ShipPickUp spu)
            {
                AddEntity(Gui.CreatePickUpNotification(spu.PickUp.Description));
                PlaySound(PickUp.PickupSound);
                RemoveEntity(spu.PickUp);
            }
        }

        public static void UpdateShipDamageTexture(int health)
        {
            var idx = (int)MapRange(health, 0, MaxHealth, 3, 1);

            if (idx != ShipDamageTextureIdx && (idx <= 3 && idx != 0))
            {
                ShipDamageTextureIdx = idx;
                GetEntity<Ship>().DamgageTexture = GetTexture2D(shipDamage[ShipType][ShipDamageTextureIdx]);
            }
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
#if DEBUG
            if (e is KeyPressed kp)
            {
                if (kp.KeyboardKey == KeyboardKey.KEY_E)
                {
                    ResetGame();
                    StartGame(LevelIdx);
                }

                if (kp.KeyboardKey == KeyboardKey.KEY_M)
                {
                    //TODO prompt to stop and exit to menu..
                    ResetGame();
                    GetEntityByTag<GuiContainer>(Gui.Tags.ShipSelection).Show();
                }
            }
#endif
        }
    }
}
