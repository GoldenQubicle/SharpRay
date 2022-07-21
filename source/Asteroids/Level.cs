using System.Diagnostics;

namespace Asteroids
{
    public record LevelData(
      string Description,
      int WinScore,
      int OnEnterSpawn,
      int OnEnterSpawnRadius,
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

            var ship = new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), MaxHealth, GetTexture2D(ships[SelectedShipType][SelectedShipColor]));
            var overlay = Gui.CreateScoreOverLay(MaxLifes, Data.WinScore);

            ship.EmitEvent += Game.OnGameEvent;
            ship.EmitEvent += overlay.OnGameEvent;

            AddEntity(ship);
            AddEntity(overlay);

            var theta = GetRandomValue(0, 360) * DEG2RAD; 

            for (var i = 1; i <= Data.OnEnterSpawn; i++)
            {
                var phi = theta + (MathF.Tau / Data.OnEnterSpawn) * i + (GetRandomValue(-128, 128) * DEG2RAD);
                SpawnNewAsteroid(PickRandomAsteroid(), phi, Data.OnEnterSpawnRadius + GetRandomValue(-20, 20), new Vector2(WindowWidth/2, WindowHeight/2));
            }

            Debug.Assert(CurrentLifes == MaxLifes);
            Debug.Assert(CurrentHealth == MaxHealth);
            Debug.Assert(CurrentScore == 0);
            Debug.Assert(PrimaryWeapon.GetStatesCount() == 1);

            IsPaused = false;
        }

        public void OnExit()
        {
            ShowCursor();
            IsPaused = true;
            CurrentScore = 0;
            CurrentLifes = MaxLifes;
            CurrentHealth = MaxHealth;
            currentTime = 0;
            StopAllSounds();
            RemoveEntitiesOfType<Ship>();
            RemoveEntitiesOfType<Bullet>();
            RemoveEntitiesOfType<Asteroid>();
            RemoveEntitiesOfType<PickUp>();
            RemoveEntity(GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay));

            AddEntity(Gui.CreateLevelWin(Data.Description), Game.OnGuiEvent);
            GetEntity<StarField>().Generate();

            if (LevelIdx == Levels.Data.Count - 1)
            {
                PlaySound(WinOverallSound);
            }
            else
            {
                PlaySound(WinSound);
            }

        }

        public void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidDestroyed ad)
            {
                // Play sound.
                SetSoundPitch(Sounds[Asteroid.ExplosionSound], GetRandomValue(50, 150) / 100f);
                PlaySound(Asteroid.ExplosionSound);

                // Update score.
                var hp = Asteroid.GetHitPoints(ad.Asteroid.Definition);
                CurrentScore += hp;
                Data.PickUps.ForEach(pu => pu.UpdateScore(hp));
                GetEntityByTag<GuiContainer>(Gui.Tags.ScoreOverlay).OnGameEvent(e);

                // Spawn new asteroids from the one destroyed.
                var spawns = Asteroid.GetSpawns(ad.Asteroid.Definition);
                foreach (var (s, i) in spawns.Select((s, i) => (s, i)))
                {
                    var angle = MathF.Tau / spawns.Count * i + (DEG2RAD * GetRandomValue(-10, 10));
                    var heading = ad.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    AddEntity(new Asteroid(s.Size, s.Type, ad.Asteroid.Position, heading), OnGameEvent);
                }

                // Determine random chance for a health pickup spawn.
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

                // Remove the asteroid.
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

            // Spawn new asteroids from outside screen bounds.
            if (currentTime > Data.MaxSpawnTime)
            {
                currentTime = 0;
                var chance = GetRandomValue(1, 100) / 100f;

                if (chance < Data.Easing.GetValue())
                {
                    var theta = GetRandomValue(0, 360) * DEG2RAD;
                    var target = GetEntity<Ship>().Position;
                    SpawnNewAsteroid(PickRandomAsteroid(), theta, WindowHeight, target);
                }
            }
        }

        private (Asteroid.Size size, Asteroid.Type type) PickRandomAsteroid() =>
            Data.AsteroidSpawnDuring[GetRandomValue(0, Data.AsteroidSpawnDuring.Count - 1)];

        private void SpawnNewAsteroid((Asteroid.Size size, Asteroid.Type type) def, float theta, int radius, Vector2 target)
        {
            var pos = new Vector2(MathF.Cos(theta) * radius, MathF.Sin(theta) * radius) + new Vector2(WindowWidth/ 2, WindowHeight/ 2);
            var heading = Vector2.Normalize(target - pos);
            heading *= Data.InitialHeadingSpeed;
            AddEntity(new Asteroid(def.size, def.type, pos, heading), OnGameEvent);
        }
    }
}
