namespace Asteroids
{
    public class Level : GameEntity
    {

        private readonly List<Asteroid> SpawnStart = new()
        {
            new Asteroid(Asteroid.Size.Large, Asteroid.Type.Dirt, new Vector2(800, 100), new Vector2(0, 1.5f)),
        };

        private readonly List<(Asteroid.Size size, Asteroid.Type type)> SpawnDuring = new()
        {
            (Asteroid.Size.Large, Asteroid.Type.Dirt),
            (Asteroid.Size.Medium, Asteroid.Type.Dirt),
            (Asteroid.Size.Tiny, Asteroid.Type.Dirt),
            (Asteroid.Size.Large, Asteroid.Type.Dirt),
            (Asteroid.Size.Medium, Asteroid.Type.Dirt),
            (Asteroid.Size.Small, Asteroid.Type.Dirt),

        };

        private double[] timings;
        private int spawnIndex = 0;
        private double currentTime = 0d;

        public Level()
        {
            foreach (var asteroid in SpawnStart)
            {
                AddEntity(asteroid, Game.OnGameEvent);
            }

            timings = Enumerable.Range(1, SpawnDuring.Count).Select(n => Easings.EaseCircOut(n, 0, 10000, 10) * SharpRayConfig.TickMultiplier).ToArray();
        }


        public override void Update(double deltaTime)
        {
            currentTime += deltaTime;

            if (spawnIndex < timings.Length && currentTime > timings[spawnIndex])
            {
                var def = SpawnDuring[spawnIndex];
                var theta = MathF.Tau / timings.Length;

                var x = MathF.Cos(spawnIndex + 1 * theta) * Game.WindowHeight;
                var y = MathF.Sin(spawnIndex + 1 * theta) * Game.WindowHeight;

                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);

                var target = GetEntity<Ship>().Position;
                var heading = Vector2.Normalize(target - pos);
                heading *= new Vector2(5, 5);

                AddEntity(new Asteroid(def.size, def.type, pos, heading), Game.OnGameEvent);
                spawnIndex++;
            }
        }
    }
}
