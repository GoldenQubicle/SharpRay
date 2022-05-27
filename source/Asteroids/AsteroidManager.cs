namespace Asteroids
{
    public class AsteroidManager : GameEntity
    {
        //texture keys
        public const string Large = "big";
        public const string Medium = "med";
        public const string Small = "small";
        public const string Tiny = "tiny";

        private const string Brown = nameof(Brown);
        private const string Grey = nameof(Grey);

        private readonly float spawnRadius = Game.WindowHeight;

        public static Dictionary<string, int> Stages { get; } = new()
        {
            { Large, 4 },
            { Medium, 3 },
            { Small, 2 },
            { Tiny, 1 }
        };

        public static Dictionary<int, Dictionary<string, int>> HitPointsPerLevelStage { get; } = new()
        {
            {
                1,
                new()
                {
                    { Large, 5 },
                    { Medium, 3 },
                    { Small, 2 },
                    { Tiny, 1 }
                }
            }
        };

        private static Dictionary<int, Dictionary<string, int>> AsteroidShatterPerLevelStage = new()
        {
            {
                1,
                new()
                {
                    { Large, 5 },
                    { Medium, 3 },
                    { Small, 2 }
                }
            }
        };

        private static Dictionary<int, int> LargeAsteroidSpawnPerLevel = new()
        {
            { 1, 2 } // 1st level has 3 total
        };

        private static Dictionary<int, double> LargestAsteroidSpawnTimePerLevel = new()
        {
            { 1, 1500 * SharpRayConfig.TickMultiplier }
        };

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
        public int Spawned { get; set; } = 1;
        public override void Update(double deltaTime)
        {
            currentTime += deltaTime;
            if(currentTime > LargestAsteroidSpawnTimePerLevel[Game.Level] 
                && Spawned <= LargeAsteroidSpawnPerLevel[Game.Level])
            {
                var theta = RAD2DEG * (360 / GetRandomValue(1, 36));
                var x = MathF.Cos(theta) * spawnRadius;
                var y = MathF.Sin(theta) * spawnRadius;

                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);

                var target = GetEntity<Ship>().Position;
                var heading = Vector2.Normalize(target - pos);
                heading *= new Vector2(1.5f, 1.5f);

                AddEntity(new Asteroid(pos, heading, Large), Game.OnGameEvent);
                currentTime = 0d;
                Spawned++;
            }
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidDestroyed ahw)
            {
                if (Stages[ahw.Asteroid.Stage] > 1)
                {
                    var nextStage = Stages[ahw.Asteroid.Stage] - 1;
                    var size = nextStage == 3 ? Medium : nextStage == 2 ? Small : Tiny;
                    var amount = AsteroidShatterPerLevelStage[Game.Level][ahw.Asteroid.Stage];
                    for (var i = 1; i <= amount; i++)
                    {
                        var angle = (MathF.Tau / amount) * i;
                        var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                        AddEntity(new Asteroid(ahw.Asteroid.Position, heading, size), Game.OnGameEvent);
                    }
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

        public static Texture2D GetRandomAsteroidTexture(string size) =>
            GetTexture2D(Game.meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? Grey : Brown;

        private static int PickAsteroidVariation(string size) =>
            size.Equals(Large) ? GetRandomValue(1, 4) : GetRandomValue(1, 2);
    }
}
