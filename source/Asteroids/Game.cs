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
global using System;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using static Asteroids.GuiEvents;
global using static Asteroids.Assets;

namespace Asteroids
{
    public static class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        //Gui Tags
        public const string GuiShipSelection = nameof(GuiShipSelection);
        public const string GuiScoreOverlay = nameof(GuiScoreOverlay);
        public const string GuiHealth = nameof(GuiHealth);
        public const string GuiScore = nameof(GuiScore);
        public const string btnStartGame = nameof(btnStartGame);
        public const string btnShipSelectRight = nameof(btnShipSelectRight);
        public const string btnShipSelectLeft = nameof(btnShipSelectLeft);
        private static string GuiLifeIcon(int n) => $"PlayerLife{n}";
        private static string GetScoreString(int s) => $"Score : {s}";
        private static string GetHealthString(int h) => $"Health : {h}";

        //Render layers
        internal const int RlBackground = 0;
        internal const int RlGuiShipSelection = 1;
        internal const int RlAsteroidsBullets = 2;
        internal const int RlShip = 3;
        internal const int RlGuiScoreOverlay = 4;

        //ship colors
        public const string Blue = "blue";
        public const string Red = "red";
        public const string Green = "green";
        public const string Orange = "orange";

        private static Dictionary<string, Color> GuiShipBaseColor = new()
        {
            { Blue, Color.DARKBLUE },
            { Red, Color.MAROON },
            { Green, Color.LIME },
            { Orange, new Color(200, 100, 0, 255) },
        };

        private static Dictionary<string, Color> GuiShipFocusColor = new()
        {
            { Blue, Color.BLUE },
            { Red, Color.RED },
            { Green, Color.GREEN },
            { Orange, Color.ORANGE },
        };

        //Game state & stats
        private static int ShipType = 3; // 1 | 2 | 3
        private static string ShipColor = Green;
        private static int ShipDamageTextureIdx = -1; // 1 | 2 | 3, initialized at -1 because reasons..
        private static int Score = 0;
        private static int Health;
        private static readonly int MaxHealth = 10;
        private static int PlayerLifes;
        private static readonly int MaxPlayerLifes = 3;

        static async Task Main(string[] args)
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = new Color(12, 24, 64, 0),
                ShowFPS = true,
                DoEventLogging = false
            });

            SetKeyBoardEventAction(OnKeyBoardEvent);
            await Load();

            File.WriteAllLines(Path.Combine(AssestsFolder, "stats.txt"), Asteroid.GetStats());

            AddEntity(new StarField());
            //AddEntity(CreateShipSelectionMenu());
            StartGame();
            Run();
        }


        public static void StartGame()
        {
            Health = MaxHealth;
            PlayerLifes = MaxPlayerLifes;

            var ship = new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[ShipType][ShipColor]));

            AddEntity(ship, OnGameEvent);
            AddEntity(new Level());

            var overlay = CreateScoreOverLay();
            ship.EmitEvent += overlay.OnGameEvent;
            overlay.Show();
            AddEntity(overlay);
        }

        private static void ResetGame()
        {
            StopAllSounds();

            //remove entities
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<Level>();
            RemoveEntity(GetEntityByTag<GuiContainer>(GuiScoreOverlay));

            //reset game stats
            Score = 0;
            Health = MaxHealth;
            PlayerLifes = MaxPlayerLifes;

            //generate new back ground
            GetEntity<StarField>().Generate();
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                RemoveEntity(sha.Asteroid);
                Health -= Asteroid.GetDamageDone(sha.Asteroid.Definition);
                GetEntityByTag<GuiContainer>(GuiScoreOverlay)
                    .GetEntityByTag<Label>(GuiHealth).Text = GetHealthString(Health);

                var idx = (int)MapRange(Health, 0, MaxHealth, 3, 1);

                if (idx != ShipDamageTextureIdx && idx <= 3)
                {
                    ShipDamageTextureIdx = idx;
                    GetEntity<Ship>().DamgageTexture = GetTexture2D(shipDamage[ShipType][ShipDamageTextureIdx]);
                }

                if (Health <= 0)
                {
                    GetEntity<Ship>().HasTakenDamage = false; // prevent damage texture from being visible
                    ShipDamageTextureIdx = -1;
                    Health = MaxHealth;

                    //grey out player life icon 
                    var overlay = GetEntityByTag<GuiContainer>(GuiScoreOverlay);
                    overlay.GetEntityByTag<ImageTexture>(GuiLifeIcon(PlayerLifes)).Color = Color.DARKGRAY;
                    overlay.GetEntityByTag<Label>(GuiHealth).Text = GetHealthString(Health);

                    PlayerLifes--; // needs to happen last otherwise we can't get the icon
                }

                if (PlayerLifes == 0)
                {
                    ResetGame();
                    StartGame();
                    //GetEntityByTag<GuiContainer>(GuiShipSelection).Show();
                }
            }

            if (e is BulletLifeTimeExpired ble)
            {
                RemoveEntity(ble.Bullet);
            }

            if (e is AsteroidDestroyed ad)
            {
                //update gui
                Score += Asteroid.GetHitPoints(ad.Asteroid.Definition);
                GetEntityByTag<GuiContainer>(GuiScoreOverlay)
                    .GetEntityByTag<Label>(GuiScore).Text = GetScoreString(Score);

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
                    GetEntityByTag<GuiContainer>(GuiShipSelection).Show();
                }
            }
        }



        private static GuiContainer CreateScoreOverLay()
        {
            //create container 
            var container = GuiContainerBuilder.CreateNew(isVisible: false, tag: GuiScoreOverlay, renderLayer: RlGuiScoreOverlay);

            //add score & health displays
            container.AddChildren(
                new Label
                {
                    Tag = GuiScore,
                    Position = new Vector2(WindowWidth - 200, 35),
                    Size = new Vector2(200, 50),
                    Text = GetScoreString(Score),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    Margins = new Vector2(10, 10)
                },
                new Label
                {
                    Tag = GuiHealth,
                    Position = new Vector2(WindowWidth - 500, 35),
                    Size = new Vector2(170, 50),
                    Text = GetHealthString(MaxHealth),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    Margins = new Vector2(10, 10)
                });

            //add player life icons
            var icon = GetTexture2D(shipsIcons[ShipType][ShipColor]);

            for (var i = 1; i <= PlayerLifes; i++)
            {
                var pos = new Vector2(icon.width + (i * icon.width * 1.5f), 10);
                container.AddChildren(
                     new ImageTexture(icon, Color.WHITE)
                     {
                         Tag = GuiLifeIcon(i),
                         Position = pos,
                     });
            }

            return container;
        }

   

        private static GuiContainer CreateShipSelectionMenu() =>
           GuiContainerBuilder.CreateNew(tag: GuiShipSelection, renderLayer: RlGuiShipSelection).AddChildren(
               new Label
               {
                   Text = "Meteor Madness",
                   TextColor = Color.YELLOW,
                   FillColor = GuiShipBaseColor[ShipColor],
                   FontSize = 45,
                   Position = new Vector2((WindowWidth / 2), WindowHeight / 8),
                   Size = new Vector2(400, 100),
                   Margins = new Vector2(35, 30),
               },
               new ImageTexture(GetTexture2D(ships[ShipType][ShipColor]), Color.WHITE)
               {
                   Position = new Vector2(WindowWidth / 2, WindowHeight / 2) -
                   new Vector2(GetTexture2D(ships[ShipType][ShipColor]).width / 2, GetTexture2D(ships[ShipType][ShipColor]).height / 2) // rather stupid tbh
               },
               new Button
               {
                   Tag = btnShipSelectLeft,
                   Position = new Vector2(WindowWidth * .2f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new ChangeShipType
                   {
                       GuiEntity = e,
                       ShipType = ShipType == 1 ? 3 : ShipType - 1
                   }
               },
               new Button
               {
                   Tag = btnShipSelectRight,
                   Position = new Vector2(WindowWidth * .8f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new ChangeShipType
                   {
                       GuiEntity = e,
                       ShipType = ShipType == 3 ? 1 : ShipType + 1
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .2f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Blue],
                   FocusColor = GuiShipFocusColor[Blue],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Blue
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .4f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Green],
                   FocusColor = GuiShipFocusColor[Green],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Green
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .6f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Red],
                   FocusColor = GuiShipFocusColor[Red],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Red
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .8f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Orange],
                   FocusColor = GuiShipFocusColor[Orange],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Orange
                   }
               },
               new Button
               {
                   Tag = btnStartGame,
                   Text = "Start",
                   TextColor = Color.YELLOW,
                   FontSize = 24,
                   Margins = new Vector2(28, 15),
                   Position = new Vector2(WindowWidth * .5f, WindowHeight * .9f),
                   Size = new Vector2(125, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new GameStart { GuiEntity = e }
               })
           .OnGuiEvent((e, c) =>
           {
               if (e is GameStart gs)
               {
                   c.Hide();
                   StartGame();
               }

               if (e is ChangeShipType cst)
               {
                   ShipType = cst.ShipType;
                   var texture = GetTexture2D(ships[ShipType][ShipColor]);
                   c.GetEntity<ImageTexture>().Texture2D = texture;
                   c.GetEntity<ImageTexture>().Position = new Vector2(WindowWidth / 2, WindowHeight / 2) - new Vector2(texture.width / 2, texture.height / 2);
               }

               if (e is ChangeShipColor csc)
               {
                   ShipColor = csc.ShipColor;
                   c.GetEntity<ImageTexture>().Texture2D = GetTexture2D(ships[ShipType][ShipColor]);
                   c.GetEntity<Label>().FillColor = GuiShipBaseColor[ShipColor];
                   c.GetEntities<Button>()
                        .Where(b => b.Tag.Equals(btnShipSelectLeft) || b.Tag.Equals(btnShipSelectRight) || b.Tag.Equals(btnStartGame)).ToList()
                        .ForEach(b =>
                           {
                               b.BaseColor = GuiShipBaseColor[ShipColor]; ;
                               b.FocusColor = GuiShipFocusColor[ShipColor]; ;
                           });
               }
           });


    }
}
