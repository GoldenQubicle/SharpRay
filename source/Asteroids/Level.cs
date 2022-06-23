namespace Asteroids
{
    public record LevelData(
      string Description,
      int WinScore,
      List<Asteroid> AsteroidSpawnStart,
      List<(Asteroid.Size size, Asteroid.Type type)> AsteroidSpawnDuring,
      Vector2 InitialHeadingSpeed,
      double MaxSpawnTime,
      Easing Easing,
      List<PickUp> PickUps);


    public class Level : Entity, IHasUpdate
    {
        public const string WinSound = nameof(WinSound);
        public LevelData Data { get; set; }

        private double currentTime = 0d;

        public void OnEnter(LevelData data)
        {
            Data = data;
            

            PrimaryWeapon.OnStartLevel();

            var ship = new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), MaxHealth, GetTexture2D(ships[ShipType][ShipColor]));
            var overlay = Gui.CreateScoreOverLay(MaxLifes, Data.WinScore);

            ship.EmitEvent += Game.OnGameEvent;
            ship.EmitEvent += overlay.OnGameEvent;

            AddEntity(ship);
            AddEntity(overlay);

            foreach (var asteroid in Data.AsteroidSpawnStart)
            {
                AddEntity(asteroid, OnGameEvent);
            }

            IsPaused = false;
        }

        public void OnExit()
        {
            ShowCursor();
            IsPaused = true;
            CurrentScore = 0;
            currentTime = 0;
            StopAllSounds();
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<PickUp>();

            AddEntity(Gui.CreateLevelWin(Data.Description), Game.OnGuiEvent);

            if (LevelIdx == Levels.Data.Count - 1)
            {
                PlaySound(WinOverallSound);
            }
            else
            {
                RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));
                PlaySound(WinSound);
            }
        }

        public void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidDestroyed ad)
            {
                //play sound
                SetSoundPitch(Sounds[Asteroid.ExplosionSound], GetRandomValue(50, 150) / 100f);
                PlaySound(Asteroid.ExplosionSound);

                //update score
                var hp = Asteroid.GetHitPoints(ad.Asteroid.Definition);
                CurrentScore += hp;
                Data.PickUps.ForEach(pu => pu.UpdateScore(hp));
                GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay).OnGameEvent(e);

                //spawn new asteroids from the one destroyed
                var spawns = Asteroid.GetSpawns(ad.Asteroid.Definition);
                foreach (var (s, i) in spawns.Select((s, i) => (s, i)))
                {
                    var angle = MathF.Tau / spawns.Count * i + (DEG2RAD * GetRandomValue(-10, 10));
                    var heading = ad.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    AddEntity(new Asteroid(s.Size, s.Type, ad.Asteroid.Position, heading), OnGameEvent);
                }

                //determine random chance for a health pickup spawn
                if (ad.Asteroid.Definition.type == Asteroid.Type.Emerald)
                {
                    var h = (int)ad.Asteroid.Definition.size;
                    var s = GetRandomValue(1, 50) / h;
                    var c = GetRandomValue(1, 25);
                    if (s > c)
                    {
                        var pu = new PickUp
                        {
                            PickupType = PickUp.Type.Health,
                            Description = $"Gained {h} health!",
                            OnPickUp = () =>
                            {
                                var ship = GetEntity<Ship>();
                                CurrentHealth += h;
                                MaxHealth = CurrentHealth > MaxHealth ? CurrentHealth : MaxHealth;
                                Gui.UpdateHealthOverlay(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay), CurrentHealth);
                                UpdateShipDamageTexture(CurrentHealth);
                            }
                        };
                        pu.OnSpawn(ad.Asteroid.Position);
                    }
                }

                // remove the asteroid 
                RemoveEntity(ad.Asteroid);
            }
        }


        public override void Update(double deltaTime)
        {
            if (IsPaused) return;

            currentTime += deltaTime;
            Data.Easing.Update(deltaTime);

            if (CurrentScore >= Data.WinScore)
                OnExit();

            foreach (var pickUp in Data.PickUps.Where(p => p.CanSpawn))
            {
                pickUp.OnSpawn(new Vector2(GetRandomValue(100, WindowWidth - 100), GetRandomValue(100, WindowHeight - 100)));
            }

            //spawn new asteroids 
            if (currentTime > Data.MaxSpawnTime)
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

                    AddEntity(new Asteroid(def.size, def.type, pos, heading), OnGameEvent);
                }
            }
        }
    }
}
