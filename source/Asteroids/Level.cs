namespace Asteroids
{
    public record LevelData(
      string Description,
      int WinScore,
      Ship.Layout ShipLayout,
      int Lifes,
      List<Asteroid> AsteroidSpawnStart,
      List<(Asteroid.Size size, Asteroid.Type type)> AsteroidSpawnDuring,
      Vector2 InitialHeadingSpeed,
      float SpawnTime,
      Func<float, float, float, float, float> Easing,
      List<PickUp> PickUps);

    public class Level : Entity, IHasUpdate
    {
        private LevelData Data { get; set; }
        private double[] timings;
        private int spawnIndex = 0;
        private double currentTime = 0d;

        public void OnEnter(LevelData data)
        {
            Data = data;

            var ship = new Ship(Data.ShipLayout, GetTexture2D(ships[ShipType][ShipColor]));
            var overlay = Gui.CreateScoreOverLay(Data.Lifes);
            var notice = Gui.CreateNotification();

            ship.EmitEvent += Game.OnGameEvent;
            ship.EmitEvent += overlay.OnGameEvent;
            ship.EmitEvent += notice.OnGameEvent;

            AddEntity(ship);
            AddEntity(overlay);
            AddEntity(notice);

            foreach (var asteroid in Data.AsteroidSpawnStart)
                AddEntity(asteroid, Game.OnGameEvent);

            timings = Enumerable.Range(1, Data.AsteroidSpawnDuring.Count)
                .Select(n => Data.Easing(n, 0, Data.SpawnTime, Data.AsteroidSpawnDuring.Count) * SharpRayConfig.TickMultiplier)
                .ToArray();

            IsPaused = false;
        }

        public void OnExit()
        {
            IsPaused = true;
            Score = 0;
            StopAllSounds();
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<PickUp>();
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.Notification));
            AddEntity(Gui.CreateLevelWin(), Game.OnGuiEvent);
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused)
            {
                return;
            }

            currentTime += deltaTime;

            if (Score >= Data.WinScore)
                OnExit();

            foreach (var pickUp in Data.PickUps)
            {
                if (Score >= pickUp.SpawnScore && !pickUp.HasSpawned)
                {
                    pickUp.OnSpawn(new Vector2(GetRandomValue(100, WindowWidth-100), GetRandomValue(100, WindowWidth-100)));
                }
            }

            if (spawnIndex < timings.Length && currentTime > timings[spawnIndex])
            {
                var def = Data.AsteroidSpawnDuring[spawnIndex];
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
