namespace Asteroids
{
    public static class Assets
    {
        public const string FontFuture = nameof(FontFuture);
        public const string FontFutureThin = nameof(FontFutureThin);

        //asteroid texture size keys
        private const string tkBig = "big";
        private const string tkMedium = "med";
        private const string tkSmall = "small";
        private const string tkTiny = "tiny";
        //asteroid texture color keys
        private const string Brown = nameof(Brown);
        private const string Grey = nameof(Grey);

        internal static Dictionary<string, Dictionary<string, Dictionary<int, string>>> meteors; //[Color][Size][Variation] 
        internal static Dictionary<int, Dictionary<Gui.ShipColor, string>> ships; // [Type][Color]
        internal static Dictionary<int, Dictionary<string, string>> shipsIcons; // [Type][Color]
        internal static Dictionary<int, Dictionary<int, string>> shipDamage; // [Type][Stage]
        internal static List<string> fireEffects;

        public const string starTexture = nameof(starTexture);
        public static Texture2D GetAsteroidTexture((Asteroid.Size size, Asteroid.Type type) a) =>
            GetTexture2D(meteors[GetAsteroidColorKey(a.type)][GetAsteroidSizeKey(a.size)][GetAsteroidVariation(a.size)]);

        public static Texture2D GetRandomAsteroidTexture(Asteroid.Size size) =>
           GetTexture2D(meteors[PickAsteroidColor()][PickAsteroidColor()][GetAsteroidVariation(size)]);

        private static string GetAsteroidColorKey(Asteroid.Type type) => type switch
        {
            Asteroid.Type.Dirt => Brown,
            Asteroid.Type.Stone => Grey,
            Asteroid.Type.Ruby => Grey,
            Asteroid.Type.Emerald => Brown,
            Asteroid.Type.Saphire => Grey,
            _ => throw new ArgumentOutOfRangeException($"No color key found for asteroid type {type}")
        };

        private static string GetAsteroidSizeKey(Asteroid.Size size) => size switch
        {
            Asteroid.Size.Big or Asteroid.Size.Large => tkBig,
            Asteroid.Size.Medium => tkMedium,
            Asteroid.Size.Small => tkSmall,
            Asteroid.Size.Tiny => tkTiny,
            _ => throw new ArgumentOutOfRangeException($"No texture key found for asteroid size {size}")
        };

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? Grey : Brown;

        private static int GetAsteroidVariation(Asteroid.Size size) =>
            GetAsteroidSizeKey(size).Equals(tkBig) ? GetRandomValue(1, 4) : GetRandomValue(1, 2);

        public static void Load()
        {
            AddFont(FontFuture, "kenvector_future.ttf");
            AddFont(FontFutureThin, "kenvector_future_thin.ttf");

            LoadSounds();

            AddTexture2D(Gui.Tags.ShipSelectLeft, $@"PNG\UI\left.png");
            AddTexture2D(Gui.Tags.ShipSelectRight, $@"PNG\UI\forward.png");
            AddTexture2D(nameof(KeyLeftDown), $@"PNG\UI\arrowLeft.png");
            AddTexture2D(nameof(KeyRightDown), $@"PNG\UI\arrowRight.png");
            AddTexture2D(nameof(KeyUpDown), $@"PNG\UI\arrowUp.png");

            var fireRegex = new Regex(@"(?<fire>fire)(?<no>\d+)");
            fireEffects = Directory.GetFiles(AssestsFolder, @"PNG\Effects\")
                .Select(f => fireRegex.Matches(f).Where(m => m.Success))
                .SelectMany(mc => mc.Select(m => m.Groups["0"].Value)).ToList();

            string getFireEffectPath(string name) => @$"PNG\Effects\{name}.png";
            foreach (var effect in fireEffects)
                AddTexture2D(effect, getFireEffectPath(effect));

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
                    g.ToDictionary(t => Enum.Parse<Gui.ShipColor>(t.color), t => t.File));

            //actually load texture into memory
            string getShipPath(string name) => @$"PNG\Ships\playerShip{name}.png";
            foreach (var s in ships.SelectMany(t => t.Value))
                AddTexture2D(s.Value, getShipPath(s.Value));

            var iconRegex = new Regex(@"(?<Type>1|2|3).(?<Color>blue|green|orange|red)");
            shipsIcons = Directory.GetFiles(AssestsFolder, @"PNG\UI\icons\")
                .Select(f => iconRegex.Match(f).Groups)
                .Select(g => (type: int.Parse(g["Type"].Value), color: g["Color"].Value, File: "icon_" + g["0"].Value))
                .GroupBy(t => t.type).ToDictionary(g => g.Key, g =>
                    g.ToDictionary(t => t.color, t => t.File));

            //actually load texture into memory
            string getIconPath(string name) => @$"PNG\UI\icons\playerLife{name[5..]}.png";
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

        private static void LoadSounds()
        {
            AddSound(Ship.EngineSound, "Audio\\spaceEngineLow_001.ogg");
            AddSound(Ship.ThrusterSound, "Audio\\thrusterFire_001.ogg");
            AddSound(Ship.HitSound, "Audio\\impactMetal_003.ogg");
            AddSound(PrimaryWeapon.SingleSound, "Audio\\laserSmall_000.ogg");
            AddSound(PrimaryWeapon.TripleSound, "Audio\\laserSmall_001.ogg");
            AddSound(PrimaryWeapon.QuintupleSound, "Audio\\laserSmall_002.ogg");
            AddSound(Asteroid.ExplosionSound, "Audio\\explosionCrunch_000.ogg");
            AddSound(Asteroid.BounceSound, "Audio\\impactMetal_002.ogg");
            SetSoundVolume(Sounds[Asteroid.BounceSound], 0.25f);
            SetSoundVolume(Sounds[Asteroid.ExplosionSound], 0.25f);
            AddSound(PickUp.PickupSound, "Audio\\sfx_shieldUp.ogg");
            AddSound(PickUp.SpawnSound, "Audio\\sfx_twoTone.ogg");
            AddSound(Gui.ButtonClickSound, "Audio\\doorOpen_002.ogg");
            SetSoundPitch(Sounds[Gui.ButtonClickSound], 6f);
            SetSoundVolume(Sounds[Gui.ButtonClickSound], 0.25f);
            AddSound(Gui.SelectionSound, "Audio\\mixkit-game-level-music-689.wav");
            SetSoundVolume(Sounds[Gui.SelectionSound], 0.25f);
            AddSound(Level.WinSound, "Audio\\mixkit-winning-an-extra-bonus-2060.wav");
            SetSoundVolume(Sounds[Level.WinSound], 0.25f);
            AddSound(LifeLostSound1, "Audio\\lowFrequency_explosion_001.ogg");
            AddSound(LifeLostSound2, "Audio\\mixkit-arcade-space-shooter-dead-notification-272.wav");
            SetSoundVolume(Sounds[LifeLostSound2], 0.5f);
            AddSound(StartSound, "Audio\\mixkit-extra-bonus-in-a-video-game-2045.wav");
            SetSoundVolume(Sounds[StartSound], 0.5f);
            AddSound(WinOverallSound, "Audio\\mixkit-game-bonus-reached-2065.wav");
            SetSoundVolume(Sounds[WinOverallSound], 0.35f);
        }
    }
}
