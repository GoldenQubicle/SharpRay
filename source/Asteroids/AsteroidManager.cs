using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asteroids
{
    public enum AsteroidSize
    {
        Tiny,
        Small,
        Medium,
        Large,
        Big
    }

    public enum AsteroidType
    {
        Dirt,
        Stone,
        Emerald,
        Ruby,
        Saphire,
    }

    public class AsteroidManager : GameEntity
    {
        //texture keys
        public const string tkBig = "big";
        public const string tkMedium = "med";
        public const string tkSmall = "small";
        public const string tkTiny = "tiny";

        private const string Brown = nameof(Brown);
        private const string Grey = nameof(Grey);

        private readonly float spawnRadius = Game.WindowHeight;

        private static int GetHitPoints(AsteroidSize size, AsteroidType type) => (size, type) switch
        {
            (AsteroidSize.Big, AsteroidType.Dirt) => 5,
            (AsteroidSize.Large, AsteroidType.Dirt) => 4,
            (AsteroidSize.Medium, AsteroidType.Dirt) => 3,
            (AsteroidSize.Small, AsteroidType.Dirt) => 2,
            (AsteroidSize.Tiny, AsteroidType.Dirt) => 1,
        };

        private static List<AsteroidData> GetSpawns(AsteroidSize size, AsteroidType type) => (size, type) switch
        {
            (AsteroidSize.Big, AsteroidType.Dirt) => new()
            {
                new AsteroidData(AsteroidSize.Large, AsteroidType.Dirt),
                new AsteroidData(AsteroidSize.Medium, AsteroidType.Dirt),
                new AsteroidData(AsteroidSize.Large, AsteroidType.Dirt),
                new AsteroidData(AsteroidSize.Medium, AsteroidType.Dirt),
                new AsteroidData(AsteroidSize.Small, AsteroidType.Dirt),
                new AsteroidData(AsteroidSize.Medium, AsteroidType.Dirt),
            },
            (AsteroidSize.Large, AsteroidType.Dirt) => new()
            {
                new AsteroidData(AsteroidSize.Medium, AsteroidType.Dirt),

            },
            (AsteroidSize.Medium, AsteroidType.Dirt) => new() { },
            (AsteroidSize.Small, AsteroidType.Dirt) => new() { },
            (AsteroidSize.Tiny, AsteroidType.Dirt) => new() { },
        };

        protected record AsteroidData(AsteroidSize Size, AsteroidType Type)
        {
            public int Hitpoints { get => GetHitPoints(Size, Type); }
            public List<AsteroidData> Spawns { get => GetSpawns(Size, Type); }
        }

        private static readonly List<AsteroidData> AsteroidDefinitions = new()
        {
            new AsteroidData(AsteroidSize.Big, AsteroidType.Dirt),

            //new AsteroidData
            //{
            //    Size = AsteroidSize.Large,
            //    Type = AsteroidType.Dirt,
            //    Hitpoints = 4,
            //    Spawns = new()
            //    {
            //        (AsteroidSize.Medium, AsteroidType.Dirt),
            //        (AsteroidSize.Small, AsteroidType.Dirt),
            //    }
            //},
            //new AsteroidData
            //{
            //    Size = AsteroidSize.Medium,
            //    Type = AsteroidType.Dirt,
            //    Hitpoints = 3,
            //    Spawns = new()
            //    {
            //        (AsteroidSize.Small, AsteroidType.Dirt),
            //        (AsteroidSize.Tiny, AsteroidType.Dirt),
            //    }
            //},
            //new AsteroidData
            //{
            //    Size = AsteroidSize.Small,
            //    Type = AsteroidType.Dirt,
            //    Hitpoints = 2,
            //    Spawns = new()
            //    {
            //        (AsteroidSize.Tiny, AsteroidType.Dirt),
            //    }
            //},
            //new AsteroidData
            //{
            //    Size = AsteroidSize.Tiny,
            //    Type = AsteroidType.Dirt,
            //    Hitpoints = 1,
            //    Spawns = new() { }
            //}
        };

        public static (Texture2D texture, float scale, Color color) GetRenderData(AsteroidSize size, AsteroidType type) => type switch
        {
            AsteroidType.Dirt => (GetAsteroidTexture(Brown, size), GetScale(size), GetColor(type)),
            AsteroidType.Stone => (GetAsteroidTexture(Grey, size), GetScale(size), GetColor(type)),
            AsteroidType.Ruby => (GetAsteroidTexture(Grey, size), GetScale(size), GetColor(type)),
            AsteroidType.Emerald => (GetAsteroidTexture(Brown, size), GetScale(size), GetColor(type)),
            AsteroidType.Saphire => (GetAsteroidTexture(Grey, size), GetScale(size), GetColor(type)),
        };

        private static Color GetColor(AsteroidType type) => type switch
        {
            AsteroidType.Dirt or AsteroidType.Stone => Color.WHITE,
            AsteroidType.Ruby => Color.MAROON,
            AsteroidType.Emerald => Color.LIME,
            AsteroidType.Saphire => Color.GOLD,
        };

        private static float GetScale(AsteroidSize size) => size switch
        {
            AsteroidSize.Large => 0.75f,
            _ => 1f
        };

        private static string GetTextureKey(AsteroidSize size) => size switch
        {
            AsteroidSize.Big or AsteroidSize.Large => tkBig,
            AsteroidSize.Medium => tkMedium,
            AsteroidSize.Small => tkSmall,
            AsteroidSize.Tiny => tkTiny,
            _ => throw new ArgumentOutOfRangeException($"No texture key found for asteroid size {size}")
        };

        public static int GetTotalHitPoints(AsteroidSize size, AsteroidType type)
        {
            var a = AsteroidDefinitions.First(a => a.Size == size && a.Type == type);
            return a.Hitpoints + a.Spawns.Sum(t => AsteroidDefinitions.First(ad => ad.Size == t.Size && ad.Type == t.Type).Hitpoints);
        }



        public AsteroidManager()
        {
            var theta = MathF.Tau / 10;
            for (var i = 0; i < 10; i++)
            {
                var x = MathF.Cos(i * theta) * spawnRadius;
                var y = MathF.Sin(i * theta) * spawnRadius;

                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);

                var target = new Vector2(150, Game.WindowHeight / 2);
                var heading = Vector2.Normalize(target - pos);
                heading *= new Vector2(5, 5);

                var defs = JsonSerializer.Serialize(AsteroidDefinitions);
                File.WriteAllText(Path.Combine(AssestsFolder, "AsteroidDefitions.json"), defs);
                //AddEntity(new Asteroid(pos, heading, Medium), Game.OnGameEvent);
            }
        }

        private double currentTime = 0d;
        public static int Spawned { get; set; } = 1;
        public override void Update(double deltaTime)
        {
            currentTime += deltaTime;
            //if(currentTime > LargestAsteroidSpawnTimePerLevel[Game.Level] 
            //    && Spawned <= LargeAsteroidSpawnPerLevel[Game.Level])
            //{
            //    var theta = RAD2DEG * (360 / GetRandomValue(1, 36));
            //    var x = MathF.Cos(theta) * spawnRadius;
            //    var y = MathF.Sin(theta) * spawnRadius;

            //    var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);

            //    var target = GetEntity<Ship>().Position;
            //    var heading = Vector2.Normalize(target - pos);
            //    heading *= new Vector2(1.5f, 1.5f);

            //    AddEntity(new Asteroid(pos, heading, Large), Game.OnGameEvent);
            //    currentTime = 0d;
            //    Spawned++;
            //}
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidDestroyed ahw)
            {
                var a = AsteroidDefinitions.First(a => ahw.Asteroid.aSize == a.Size && ahw.Asteroid.aType == a.Type);

                foreach (var (s, i) in a.Spawns.Select((s, i) => (s, i)))
                {
                    var angle = (MathF.Tau / a.Spawns.Count) * i;
                    var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    var hp = AsteroidDefinitions.First(ad => ad.Size == s.Size && ad.Type == s.Type).Hitpoints;
                    AddEntity(new Asteroid(s.Size, s.Type, hp, ahw.Asteroid.Position, heading), Game.OnGameEvent);
                }

                //if (Stages[ahw.Asteroid.Stage] > 1)
                //{
                //    var nextStage = Stages[ahw.Asteroid.Stage] - 1;
                //    var size = nextStage == 3 ? tkMedium : nextStage == 2 ? tkSmall : tkTiny;
                //    var amount = AsteroidShatterPerLevelStage[Game.Level][ahw.Asteroid.Stage];
                //    for (var i = 1; i <= amount; i++)
                //    {
                //        var angle = (MathF.Tau / amount) * i;
                //        var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                //        AddEntity(new Asteroid(ahw.Asteroid.Position, heading, size), Game.OnGameEvent);
                //    }
                //}
                RemoveEntity(ahw.Asteroid);
            }
        }



        public override void Render()
        {
            var theta = MathF.Tau / 10;
            for (var i = 0; i < 10; i++)
            {
                var x = MathF.Cos(i * theta) * spawnRadius;
                var y = MathF.Sin(i * theta) * spawnRadius;
                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);
                DrawCircleV(pos, 5, Color.BLUE);
                DrawText(i.ToString(), (int)pos.X, (int)pos.Y, 10, Color.RAYWHITE);
            }
        }

        public static Texture2D GetAsteroidTexture(string color, AsteroidSize size) =>
            GetTexture2D(Game.meteors[color][GetTextureKey(size)][PickAsteroidVariation(GetTextureKey(size))]);

        public static Texture2D GetRandomAsteroidTexture(string size) =>
            GetTexture2D(Game.meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? Grey : Brown;

        private static int PickAsteroidVariation(string size) =>
            size.Equals(tkBig) ? GetRandomValue(1, 4) : GetRandomValue(1, 2);
    }
}
