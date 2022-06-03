namespace Asteroids
{
    public record LevelData(
      List<Asteroid> SpawnStart,
      List<(Asteroid.Size size, Asteroid.Type type)> SpawnDuring,
      Vector2 InitialHeadingSpeed,
      float SpawnTime,
      Func<float, float, float, float, float> Easing,
      List<PickUp> PickUps);

    public class Level : Entity, IHasUpdate
    {
        private LevelData Data { get; }
        private double[] timings;
        private int spawnIndex = 0;
        private double currentTime = 0d;

        public Level(LevelData data)
        {
            Data = data;
            foreach (var asteroid in Data.SpawnStart)
                AddEntity(asteroid, Game.OnGameEvent);

            timings = Enumerable.Range(1, Data.SpawnDuring.Count)
                .Select(n => Data.Easing(n, 0, Data.SpawnTime, Data.SpawnDuring.Count) * SharpRayConfig.TickMultiplier)
                .ToArray();
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused)
            {
                return;
            }

            currentTime += deltaTime;

            foreach (var pickUp in Data.PickUps)
            {
                if (Score >= pickUp.SpawnScore && !pickUp.HasSpawned)
                {
                    pickUp.OnSpawn(new Vector2(600, 200));
                }
            }

            if (spawnIndex < timings.Length && currentTime > timings[spawnIndex])
            {
                var def = Data.SpawnDuring[spawnIndex];
                var theta = MathF.Tau / timings.Length;

                var x = MathF.Cos(spawnIndex + 1 * theta) * WindowHeight;
                var y = MathF.Sin(spawnIndex + 1 * theta) * WindowHeight;

                var pos = new Vector2(x, y) + new Vector2(WindowWidth / 2, WindowHeight / 2);

                var target = GetEntity<Ship>().Position;
                var heading = Vector2.Normalize(target - pos);
                heading *= Data.InitialHeadingSpeed;

                AddEntity(new Asteroid(def.size, def.type, pos, heading), Game.OnGameEvent);
                spawnIndex++;
            }
        }
    }
}
