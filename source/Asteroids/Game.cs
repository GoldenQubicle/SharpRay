﻿using System.Numerics;
using SharpRay.Core;
using SharpRay.Eventing;
using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace Asteroids
{
    public static class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        private const string PNG = nameof(PNG);

        private static Dictionary<string, Dictionary<string, Dictionary<int, string>>> meteors;
        private static Dictionary<int, Dictionary<string, string>> ships;
        private static Dictionary<int, Dictionary<int, string>> damage;
        public const string damageTexture = "ship2damage1";

        public const string starTexture = nameof(starTexture);
        public const string shipTexture = nameof(shipTexture);
        public const string shipDamageTexture = nameof(shipDamageTexture);

        private static int ShipType = 3; // 1 | 2 | 3
        private const string ShipColor = "green"; // blue | green | red
        private static int ShipDamage = 0;

        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                ShowFPS = true,
                BackGroundColor = new Color(12, 24, 64, 0),
                DoEventLogging = true
            });

            LoadAssets();

            AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[ShipType][ShipColor])), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(800, 100), new Vector2(0, -1.5f), 4, GetTexture2D(meteors["Grey"]["big"][1])), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 4, GetTexture2D(meteors["Grey"]["tiny"][2])), OnGameEvent);
            AddEntity(new StarFieldGenerator());

            Run();
        }



        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipHitAsteroid sha)
            {
                ShipDamage++;
                if (ShipDamage >= 4) return;

                RemoveEntity(sha.Asteroid);
                GetEntity<Ship>().DamgageTexture = GetTexture2D(damage[ShipType][ShipDamage]);
            }

            if (e is ShipFiredBullet sfb)
                AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), OnGameEvent);

            if (e is BulletLifeTimeExpired ble)
                RemoveEntity(ble.Bullet);

            if (e is BulletHitAsteroid bha)
                RemoveEntity(bha.Bullet);

            if (e is AsteroidDestroyed ad)
                RemoveEntity(ad.Asteroid);

            if (e is AsteroidSpawnNew asn)
            {
                var size = asn.Stage == 3 ? "med" : asn.Stage == 2 ? "small" : "tiny";
                var amount = asn.Stage == 3 ? 7 : asn.Stage == 2 ? 5 : 3;

                for (var i = 1; i <= amount; i++)
                {
                    var angle = (MathF.Tau / amount) * i;
                    var heading = asn.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    AddEntity(new Asteroid(asn.Position, heading, asn.Stage, GetRandomAsteroidTexture(size)), OnGameEvent);
                }
            }
        }

        public static void OnGuiEvent(IGuiEvent e)
        {
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed kp)
            {
                if(kp.KeyboardKey == KeyboardKey.KEY_Q)
                {
                    ShipType = ShipType == 3 ? 1 : ShipType + 1;
                    var ship = GetEntity<Ship>();
                    ship.ShipTexture = GetTexture2D(ships[ShipType][ShipColor]);
                    if(ShipDamage > 0 && ShipDamage < 4)
                        ship.DamgageTexture = GetTexture2D(damage[ShipType][ShipDamage]);

                }

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
            }
        }



        private static Texture2D GetRandomAsteroidTexture(string size) =>
            GetTexture2D(meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? "Grey" : "Brown";

        private static int PickAsteroidVariation(string size) =>
            size.Equals("big") ? GetRandomValue(1, 4) : GetRandomValue(1, 2);

        private static void LoadAssets()
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

            //fill damage dictionary by [Type][Stage] => name with which to retrieve it with GetTexture2D
            var damageRegex = new Regex(@"(?<Type>playerShip(1|2|3)).(?<Stage>damage(1|2|3))");
            damage = Directory.GetFiles(AssestsFolder, @"PNG\Damage\")
                    .Select(f => damageRegex.Match(f).Groups)
                    .Select(g => (type: int.Parse(g["Type"].Value.Last().ToString()), stage: int.Parse(g["Stage"].Value.Last().ToString()), file: g["0"].Value))
                    .GroupBy(t => t.type).ToDictionary(g => g.Key, g =>
                        g.ToDictionary(t => t.stage, t => t.file));

            //actually load texture into memory
            string getDamagePath(string name) => @$"PNG\Damage\{name}.png";
            foreach (var s in damage.SelectMany(t => t.Value))
                AddTexture2D(s.Value, getDamagePath(s.Value));


            AddTexture2D(starTexture, $@"PNG\star_extra_small.png");
        }
    }
}
