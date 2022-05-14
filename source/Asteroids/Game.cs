using System.Numerics;
using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Gui;
using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
using System.Threading.Tasks;
using static Asteroids.GuiEvents;

namespace Asteroids
{
    public static class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        //Tags
        private const string GuiShipSelection = nameof(GuiShipSelection);
        private const string GuiScoreOverlay = nameof(GuiScoreOverlay);
        private const string GuiHealth = nameof(GuiHealth);
        private const string GuiScore = nameof(GuiScore);
        private static string GuiLifeIcon(int n) => $"PlayerLife{n}";
        private static string GetScoreString(int s) => $"Score : {s}";
        private static string GetHealthString(int h) => $"Health : {h}";

        //Render layers
        internal const int RlBackground = 0;
        internal const int RlGuiShipSelection = 1;
        internal const int RlAsteroidsBullets = 2;
        internal const int RlShip = 3;
        internal const int RlGuiScoreOverlay = 4;

        //Assets 
        private static Dictionary<string, Dictionary<string, Dictionary<int, string>>> meteors;
        private static Dictionary<int, Dictionary<string, string>> ships;
        private static Dictionary<int, Dictionary<string, string>> shipsIcons;
        private static Dictionary<int, Dictionary<int, string>> shipDamage;
        
        public const string starTexture = nameof(starTexture);

        //Game state & stats
        private static int ShipType = 3; // 1 | 2 | 3
        private static string ShipColor = "green"; // blue | green | red | orange
        private static int ShipDamageTextureIdx = -1; // 1 | 2 | 3, initialized at -1 because reasons..
        private static int Score = 0;
        private static int Health;
        private static readonly int MaxHealth = 15;
        private static int PlayerLifes;
        private static readonly int MaxPlayerLifes = 3;


        static async Task Main(string[] args)
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = new Color(12, 24, 64, 0),
                ShowFPS = false,
                DoEventLogging = false
            });

            SetKeyBoardEventAction(OnKeyBoardEvent);
            await LoadAssets();

            AddEntity(new StarFieldGenerator());
            AddEntity(CreateShipSelectionMenu());

            Run();
        }


        public static void StartGame()
        {
            Health = MaxHealth;
            PlayerLifes = MaxPlayerLifes;

            var ship = new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[ShipType][ShipColor]));

            AddEntity(ship, OnGameEvent);
            AddEntity(new Asteroid(new Vector2(800, 100), new Vector2(0, -1.5f), 4, GetTexture2D(meteors["Grey"]["big"][1])), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 4, GetTexture2D(meteors["Grey"]["tiny"][2])), OnGameEvent);


            var overlay = CreateScoreOverLay();
            ship.EmitEvent += overlay.OnGameEvent;
            overlay.Show();
            AddEntity(overlay);
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                RemoveEntity(sha.Asteroid);
                Health -= sha.Asteroid.Stage * 2;
                GetEntityByTag<GuiContainer>(GuiScoreOverlay)
                    .GetEntityByTag<Label>(GuiHealth).Text = GetHealthString(Health);

                var idx = (int)MapRange(Health, 0, MaxHealth, 3, 1);

                if (idx != ShipDamageTextureIdx)
                {
                    ShipDamageTextureIdx = idx;
                    GetEntity<Ship>().DamgageTexture = GetTexture2D(shipDamage[ShipType][ShipDamageTextureIdx]);
                }

                if (Health <= 0)
                {
                    //grey out player life icon 
                    var overlay = GetEntityByTag<GuiContainer>(GuiScoreOverlay);
                    overlay.GetEntityByTag<ImageTexture>(GuiLifeIcon(PlayerLifes)).Color = Color.DARKGRAY;
                    overlay.GetEntityByTag<Label>(GuiHealth).Text = GetHealthString(Health);

                    GetEntity<Ship>().HasTakenDamage = false; // prevent damage texture from being visible
                    ShipDamageTextureIdx = -1; 
                    Health = MaxHealth;
                    PlayerLifes--; // needs to happen last otherwise we can't get the icon
                }

                if (PlayerLifes == 0)
                {
                    ResetGame();
                    GetEntityByTag<GuiContainer>(GuiShipSelection).Show();
                }
            }

            if (e is ShipFiredBullet sfb)
            {
                var bullet = new Bullet(sfb.Origin, sfb.Angle, sfb.Force);
                bullet.EmitEvent += GetEntityByTag<GuiContainer>(GuiScoreOverlay).OnGameEvent;
                AddEntity(bullet, OnGameEvent);
            }

            if (e is BulletLifeTimeExpired ble)
            {
                RemoveEntity(ble.Bullet);
            }

            if (e is AsteroidHitByWeapon ahw)
            {
                Score += ahw.Asteroid.Stage;
                GetEntityByTag<GuiContainer>(GuiScoreOverlay)
                    .GetEntityByTag<Label>(GuiScore).Text = GetScoreString(Score);

                if (ahw.Asteroid.Stage > 1)
                {
                    var stage = ahw.Asteroid.Stage - 1;
                    var size = stage == 3 ? "med" : stage == 2 ? "small" : "tiny";
                    var amount = stage == 3 ? 7 : stage == 2 ? 5 : 3;
                    for (var i = 1; i <= amount; i++)
                    {
                        var angle = (MathF.Tau / amount) * i;
                        var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                        AddEntity(new Asteroid(ahw.Asteroid.Position, heading, stage, GetRandomAsteroidTexture(size)), OnGameEvent);
                    }
                }

                RemoveEntity(ahw.Bullet);
                RemoveEntity(ahw.Asteroid);
            }
        }

        private static void ResetGame()
        {
            //remove entities
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntity(GetEntityByTag<GuiContainer>(GuiScoreOverlay));

            //reset game stats
            Score = 0;
            Health = MaxHealth;
            PlayerLifes = MaxPlayerLifes;
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed kp)
            {
                if (kp.KeyboardKey == KeyboardKey.KEY_E)
                {
                    RemoveEntitiesOfType<Asteroid>();
                    RemoveEntitiesOfType<Ship>();
                    AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[ShipType][ShipColor])), OnGameEvent);
                    AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(.5f, 0), 4, GetRandomAsteroidTexture("big")), OnGameEvent);
                    AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 2, GetRandomAsteroidTexture("tiny")), OnGameEvent);
                    AddEntity(new Asteroid(new Vector2(800, 500), new Vector2(-.05f, -.5f), 2, GetRandomAsteroidTexture("med")), OnGameEvent);
                    AddEntity(new Asteroid(new Vector2(500, 500), new Vector2(.75f, 1.5f), 2, GetRandomAsteroidTexture("small")), OnGameEvent);
                }

                if (kp.KeyboardKey == KeyboardKey.KEY_M)
                {
                    //TODO prompt to stop and exit to menu..
                    ResetGame();
                    GetEntityByTag<GuiContainer>(GuiShipSelection).Show();
                }
            }
        }

        private static Texture2D GetRandomAsteroidTexture(string size) =>
            GetTexture2D(meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? "Grey" : "Brown";

        private static int PickAsteroidVariation(string size) =>
            size.Equals("big") ? GetRandomValue(1, 4) : GetRandomValue(1, 2);

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
                    Size = new Vector2(170, 50),
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
                   FillColor = Color.LIME,
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
                   Tag = "left",
                   Position = new Vector2(WindowWidth * .2f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = Color.LIME,
                   FocusColor = Color.GREEN,
                   OnMouseLeftClick = e => new ChangeShipType
                   {
                       GuiEntity = e,
                       ShipType = ShipType == 1 ? 3 : ShipType - 1
                   }
               },
               new Button
               {
                   Tag = "right",
                   Position = new Vector2(WindowWidth * .8f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = Color.LIME,
                   FocusColor = Color.GREEN,
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
                   BaseColor = Color.DARKBLUE,
                   FocusColor = Color.BLUE,
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = "blue"
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .4f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = Color.LIME,
                   FocusColor = Color.GREEN,
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = "green"
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .6f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = Color.MAROON,
                   FocusColor = Color.RED,
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = "red"
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .8f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = new Color(200, 100, 0, 255),
                   FocusColor = Color.ORANGE,
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = "orange"
                   }
               },
               new Button
               {
                   Tag = "start",
                   Text = "Start",
                   TextColor = Color.YELLOW,
                   FontSize = 24,
                   Margins = new Vector2(28, 15),
                   Position = new Vector2(WindowWidth * .5f, WindowHeight * .9f),
                   Size = new Vector2(125, 50),
                   BaseColor = Color.LIME,
                   FocusColor = Color.GREEN,
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
                   var color = ShipColor == "blue" ? Color.DARKBLUE
                              : ShipColor == "red" ? Color.MAROON
                              : ShipColor == "orange" ? Color.ORANGE : Color.LIME;

                   var focusColor = ShipColor == "blue" ? Color.BLUE
                                    : ShipColor == "red" ? Color.RED
                                    : ShipColor == "orange" ? Color.ORANGE : Color.GREEN;

                   c.GetEntity<Label>().FillColor = color;
                   c.GetEntities<Button>()
                        .Where(b => b.Tag == "left" || b.Tag == "right" || b.Tag == "start").ToList()
                        .ForEach(b =>
                           {
                               b.BaseColor = color;
                               b.FocusColor = focusColor;
                           });
               }
           });

        private static async Task LoadAssets()
        {
            AddSound(Ship.EngineSound, "spaceEngineLow_001.ogg");
            AddSound(Ship.ThrusterSound, "thrusterFire_001.ogg");

            //fill meteor dictionary by [Color][Size][Variation] => name with which to retrieve it with GetTexture2D
            var meteorRegex = new Regex(@"(?<Color>Brown|Grey).(?<Size>big|med|small|tiny)*(?<Variation>1|2|3|4)");
            meteors = Directory.GetFiles(AssestsFolder, @"PNG\Meteors\")
                 .Select(f => meteorRegex.Match(f).Groups)
                 .Select(g => (color: g["Color"].Value, size: g["Size"].Value, variation: int.Parse(g["Variation"].Value), file: g["0"].Value))
                 .GroupBy(t => t.color).ToDictionary(g => g.Key, g =>
                     g.GroupBy(t => t.size).ToDictionary(g => g.Key, g =>
                        g.ToDictionary(t => t.variation, t => t.file)));


            //actually load texture into memory
            string getMeteorPath(string name) => @$"PNG\Meteors\meteor{name}.png";
            foreach (var m in meteors.SelectMany(c => c.Value.SelectMany(s => s.Value)))
                AddTexture2D(m.Value, getMeteorPath(m.Value));


            //fill ship dictionary by [Type][Color] => name with which to retrieve it with GetTexture2D
            var shipRegex = new Regex(@"(?<Type>1|2|3|).(?<Color>blue|green|orange|red)");
            ships = Directory.GetFiles(AssestsFolder, @"PNG\Ships\")
                .Select(f => shipRegex.Match(f).Groups)
                .Select(g => (type: int.Parse(g["Type"].Value), color: g["Color"].Value, File: g["0"].Value))
                .GroupBy(t => t.type).ToDictionary(g => g.Key, g =>
                    g.ToDictionary(t => t.color, t => t.File));

            //actually load texture into memory
            string getShipPath(string name) => @$"PNG\Ships\playerShip{name}.png";
            foreach (var s in ships.SelectMany(t => t.Value))
                AddTexture2D(s.Value, getShipPath(s.Value));

            var iconRegex = new Regex(@"(?<Type>1|2|3).(?<Color>blue|green|orange|red)");
            shipsIcons = Directory.GetFiles(AssestsFolder, @"PNG\UI\")
                .Select(f => iconRegex.Match(f).Groups)
                .Select(g => (type: int.Parse(g["Type"].Value), color: g["Color"].Value, File: "icon_" + g["0"].Value))
                .GroupBy(t => t.type).ToDictionary(g => g.Key, g =>
                    g.ToDictionary(t => t.color, t => t.File));

            //actually load texture into memory
            string getIconPath(string name) => @$"PNG\UI\playerLife{name[5..]}.png";
            foreach (var s in shipsIcons.SelectMany(t => t.Value))
                AddTexture2D(s.Value, getIconPath(s.Value));


            //fill damage dictionary by [Type][Stage] => name with which to retrieve it with GetTexture2D
            var damageRegex = new Regex(@"(?<Type>playerShip(1|2|3)).(?<Stage>damage(1|2|3))");
            shipDamage = Directory.GetFiles(AssestsFolder, @"PNG\Damage\")
                    .Select(f => damageRegex.Match(f).Groups)
                    .Select(g => (type: int.Parse(g["Type"].Value.Last().ToString()), stage: int.Parse(g["Stage"].Value.Last().ToString()), file: g["0"].Value))
                    .GroupBy(t => t.type).ToDictionary(g => g.Key, g =>
                        g.ToDictionary(t => t.stage, t => t.file));

            //actually load texture into memory
            string getDamagePath(string name) => @$"PNG\Damage\{name}.png";
            foreach (var s in shipDamage.SelectMany(t => t.Value))
                AddTexture2D(s.Value, getDamagePath(s.Value));


            AddTexture2D(starTexture, $@"PNG\star_extra_small.png");
        }
    }
}
