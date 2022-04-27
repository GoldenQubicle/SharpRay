using System.Numerics;
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

        private static readonly Dictionary<string, Dictionary<string, Dictionary<int, string>>> meteors = new();
        private static readonly Dictionary<int, Dictionary<string, string>> ships = new();
        public const string damageTexture = "ship2damage1";
        
        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);
            Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight, ShowFPS = true });

            LoadAssets();

            AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), GetTexture2D(ships[2]["red"])), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(800, 100), new Vector2(0, -1.5f), 4, GetTexture2D(meteors["Grey"]["big"][1])), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 4, GetTexture2D(meteors["Grey"]["tiny"][2])), OnGameEvent);
            AddEntity(new Star(new Vector2(200, 500)));

            Run();
        }

        private static void LoadAssets()
        {
            AddSound(Ship.EngineSound, "spaceEngineLow_001.ogg");
            AddSound(Ship.ThrusterSound, "thrusterFire_001.ogg");

            //fill meteor dictionary by [Color][Size][Variation] => name with which to retrieve it with GetTexture2D
            var meteorRegex = new Regex(@"(?<Color>Brown|Grey).(?<Size>big|med|small|tiny)*(?<Variation>1|2|3|4)");
            Directory.GetFiles(AssestsFolder, @"PNG\Meteors\")
                .Select(Path.GetFileNameWithoutExtension)
                .Select(f => meteorRegex.Match(f).Groups)
                .Select(g => (color: g["Color"].Value, size: g["Size"].Value, variation: int.Parse(g["Variation"].Value), file: g["0"].Value)).ToList()
                .ForEach(c =>
                {
                    if (!meteors.ContainsKey(c.color)) meteors.Add(c.color, new Dictionary<string, Dictionary<int, string>>());
                    if (!meteors[c.color].ContainsKey(c.size)) meteors[c.color].Add(c.size, new Dictionary<int, string>());
                    meteors[c.color][c.size].Add(c.variation, c.file);
                });

            //actually load texture into memory
            string getMeteorPath(string name) => @$"PNG\Meteors\meteor{name}.png";
            foreach (var m in meteors.SelectMany(c => c.Value.SelectMany(s => s.Value)))
                LoadTexture2D(m.Value, getMeteorPath(m.Value));


            //fill ship dictionary by [Type][Color] => name with which to retrieve it with GetTexture2D
            var shipRegex = new Regex(@"(?<Type>1|2|3|).(?<Color>blue|green|orange|red)");
            Directory.GetFiles(AssestsFolder, @"PNG\Ships\")
                .Select(Path.GetFileNameWithoutExtension)
                .Select(f => shipRegex.Match(f).Groups)
                .Select(g => (type: int.Parse(g["Type"].Value), color: g["Color"].Value, File: g["0"].Value)).ToList()
                .ForEach(c =>
                {
                    if (!ships.ContainsKey(c.type)) ships.Add(c.type, new Dictionary<string, string> { { c.color, c.File } });
                    else ships[c.type].Add(c.color, c.File);

                });

            //actually load texture into memory
            string getShipPath(string name) => @$"PNG\Ships\playerShip{name}.png";
            foreach (var s in ships.SelectMany(t => t.Value))
                LoadTexture2D(s.Value, getShipPath(s.Value));

            LoadTexture2D(damageTexture, $@"PNG\Damage\playerShip2_damage3.png");

            LoadTexture2D("starTexture", $@"PNG\star_v1.png");
        }

        public static void OnGameEvent(IGameEvent e)
        {

            if (e is ShipHitAsteroid sha)
            {
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
            if (e is KeyPressed kp && kp.KeyboardKey == KeyboardKey.KEY_E)
            {
                RemoveEntitiesOfType<Asteroid>();
                AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(.5f, 0), 4, GetRandomAsteroidTexture("big")), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 2, GetRandomAsteroidTexture("tiny")), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(800, 500), new Vector2(-.05f, -.5f), 2, GetRandomAsteroidTexture("med")), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(500, 500), new Vector2(.75f, 1.5f), 2, GetRandomAsteroidTexture("small")), OnGameEvent);
            }
        }

        private static Texture2D GetRandomAsteroidTexture(string size) => GetTexture2D(meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);
        private static string PickAsteroidColor() => GetRandomValue(0, 1) == 1 ? "Grey" : "Brown";
        private static int PickAsteroidVariation(string size) => size.Equals("big") ? GetRandomValue(1, 4) : GetRandomValue(1, 2);

    }
}
