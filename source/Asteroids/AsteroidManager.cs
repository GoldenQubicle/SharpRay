namespace Asteroids
{
    public class AsteroidManager : GameEntity
    {
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

        public static Dictionary<string, int> HitPoints { get; } = new()
        {
            { Large, 8 },
            { Medium, 5 },
            { Small, 3 },
            { Tiny, 1 }
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

                AddEntity(new Asteroid(pos, heading, Medium), Game.OnGameEvent);
            }
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidDestroyed ahw)
            {
                if (Stages[ahw.Asteroid.Stage] > 1)
                {
                    var stage = Stages[ahw.Asteroid.Stage] - 1;
                    var size = stage == 3 ? Medium : stage == 2 ? Small : Tiny;
                    var amount = stage == 3 ? 7 : stage == 2 ? 5 : 3;
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
