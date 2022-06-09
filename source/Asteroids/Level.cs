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
      double MaxSpawnTime,
      Easing Easing,
      List<PickUp> PickUps);

    public class Level : Entity, IHasUpdate
    {
        public const string WinSound = nameof(WinSound);

        public int PickUpScore { get; set; }
        public LevelData Data { get; set; }
        private double currentTime = 0d;

        public void OnEnter(LevelData data)
        {
            Data = data;

            var ship = new Ship(Data.ShipLayout, GetTexture2D(ships[ShipType][ShipColor]));
            var overlay = Gui.CreateScoreOverLay(Data.Lifes);

            ship.EmitEvent += Game.OnGameEvent;
            ship.EmitEvent += overlay.OnGameEvent;

            AddEntity(ship);
            AddEntity(overlay);

            foreach (var asteroid in Data.AsteroidSpawnStart)
                AddEntity(asteroid, Game.OnGameEvent);
                  
            IsPaused = false;
        }

        public void OnExit()
        {
            IsPaused = true;
            Score = 0;
            currentTime = 0;
            PickUpScore = 0;
            StopAllSounds();
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<PickUp>();
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));
            AddEntity(Gui.CreateLevelWin(Data.Description), Game.OnGuiEvent);
            PlaySound(Sounds[WinSound]);
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused) return;

            currentTime += deltaTime;
            Data.Easing.Update(deltaTime);

            if (Score >= Data.WinScore)
                OnExit();

            foreach (var pickUp in Data.PickUps)
            {
                if (PickUpScore >= pickUp.SpawnScore && !pickUp.HasSpawned)
                {
                    pickUp.OnSpawn(new Vector2(GetRandomValue(100, WindowWidth-100), GetRandomValue(100, WindowHeight-100)));
                }
            }

            if(currentTime > Data.MaxSpawnTime)
            {
                currentTime = 0;
                var chance = GetRandomValue(1, 100) / 100f;
                
                if (chance < Data.Easing.GetValue())
                {
                    var idx = GetRandomValue(0, Data.AsteroidSpawnDuring.Count - 1);
                    var def = Data.AsteroidSpawnDuring[idx];
                    var theta = GetRandomValue(0, 360) * DEG2RAD;

                    var x = MathF.Cos(theta) * WindowHeight;
                    var y = MathF.Sin(theta) * WindowHeight;

                    var pos = new Vector2(x, y) + new Vector2(WindowWidth / 2, WindowHeight / 2);

                    var target = GetEntity<Ship>().Position;
                    var heading = Vector2.Normalize(target - pos);
                    heading *= Data.InitialHeadingSpeed;

                    AddEntity(new Asteroid(def.size, def.type, pos, heading), Game.OnGameEvent);
                }
            }
        }
    }
}
