namespace Asteroids
{
    public static class Assets
    {
        //asteroid texture keys
        private const string tkBig = "big";
        private const string tkMedium = "med";
        private const string tkSmall = "small";
        private const string tkTiny = "tiny";

        private const string Brown = nameof(Brown);
        private const string Grey = nameof(Grey);

    

        internal static Dictionary<string, Dictionary<string, Dictionary<int, string>>> meteors; //[Color][Size][Variation] 
        internal static Dictionary<int, Dictionary<string, string>> ships; // [Type][Color]
        internal static Dictionary<int, Dictionary<string, string>> shipsIcons; // [Type][Color]
        internal static Dictionary<int, Dictionary<int, string>> shipDamage; // [Type][Stage]

        public const string starTexture = nameof(starTexture);
        public static Texture2D GetAsteroidTexture((Asteroid.Size size, Asteroid.Type type) a) =>
            GetTexture2D(meteors[GetAsteroidColorKey(a.type)][GetTextureKey(a.size)][PickAsteroidVariation(GetTextureKey(a.size))]);

        public static Texture2D GetRandomAsteroidTexture(string size) =>
           GetTexture2D(meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string GetAsteroidColorKey(Asteroid.Type type) => type switch
        {
            Asteroid.Type.Dirt => Brown,
            Asteroid.Type.Stone => Grey,
            Asteroid.Type.Ruby => Grey,
            Asteroid.Type.Emerald => Brown,
            Asteroid.Type.Saphire => Grey,
        };

        private static string GetTextureKey(Asteroid.Size size) => size switch
        {
            Asteroid.Size.Big or Asteroid.Size.Large => tkBig,
            Asteroid.Size.Medium => tkMedium,
            Asteroid.Size.Small => tkSmall,
            Asteroid.Size.Tiny => tkTiny,
            _ => throw new ArgumentOutOfRangeException($"No texture key found for asteroid size {size}")
        };


        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? Grey : Brown;

        private static int PickAsteroidVariation(string size) =>
            size.Equals(tkBig) ? GetRandomValue(1, 4) : GetRandomValue(1, 2);


        public static async Task Load()
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
