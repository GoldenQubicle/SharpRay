namespace Asteroids
{
    public enum AsteroidSize
    {
        Tiny = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
        Big = 5
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

        public static int GetHitPoints(AsteroidSize size, AsteroidType type) => (size, type) switch
        {
            (_, AsteroidType.Dirt) => (int)size,
            (_, AsteroidType.Stone) => (int)size * 2,
            (_, AsteroidType.Emerald) => (int)size * 3,
            (_, AsteroidType.Ruby) => (int)size * 4,
            (_, AsteroidType.Saphire) => (int)size * 5
        };

        private static List<(AsteroidSize Size, AsteroidType Type)> GetSpawns(AsteroidSize size, AsteroidType type) => (size, type) switch
        {
            (_, AsteroidType.Dirt) => size switch
            {
                AsteroidSize.Big => new()
                {
                    (AsteroidSize.Large, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Large, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Large, AsteroidType.Dirt),
                },
                AsteroidSize.Large => new()
                {
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Dirt),

                },
                AsteroidSize.Medium => new() 
                {
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                },
                AsteroidSize.Small => new()
                {
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                },
                AsteroidSize.Tiny => new() { },
            },

            (_, AsteroidType.Stone) => size switch
            {
                AsteroidSize.Big => new()
                {
                    (AsteroidSize.Large, AsteroidType.Stone),
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Large, AsteroidType.Stone),
                    (AsteroidSize.Medium, AsteroidType.Dirt),
                    (AsteroidSize.Large, AsteroidType.Stone),
                },
                AsteroidSize.Large => new()
                {
                    (AsteroidSize.Medium, AsteroidType.Stone),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Stone),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Medium, AsteroidType.Stone),

                },
                AsteroidSize.Medium => new()
                {
                    (AsteroidSize.Small, AsteroidType.Stone),
                    (AsteroidSize.Small, AsteroidType.Dirt),
                    (AsteroidSize.Tiny, AsteroidType.Stone),
                },
                AsteroidSize.Small => new()
                {
                    (AsteroidSize.Tiny, AsteroidType.Stone),
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                },
                AsteroidSize.Tiny => new() 
                {
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                    (AsteroidSize.Tiny, AsteroidType.Dirt),
                },
            }

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

        public static int GetTotalHitPoints(AsteroidSize size, AsteroidType type) =>
               GetHitPoints(size, type) + GetSpawns(size, type).Sum(t => GetHitPoints(t.Size, t.Type));


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
                var spawns = GetSpawns(ahw.Asteroid.aSize, ahw.Asteroid.aType);

                foreach (var (s, i) in spawns.Select((s, i) => (s, i)))
                {
                    var angle = (MathF.Tau / spawns.Count) * i;
                    var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    AddEntity(new Asteroid(s.Size, s.Type, ahw.Asteroid.Position, heading), Game.OnGameEvent);
                }

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
