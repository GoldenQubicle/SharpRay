namespace Asteroids
{
    internal class Levels
    {
#if DEBUG
        public static List<LevelData> Data => new()
        {
           Level1, Level2, Level3, TestLevel
        };
#endif
#if RELEASE
        public static List<LevelData> Data => new()
        {
           Level1, Level2, Level3
        };
#endif

        public static LevelData Level1 => new(
            Description: "Level 1",
            WinScore: 100,
            OnEnterSpawn: 2,
            OnEnterSpawnRadius: WindowWidth / 4,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
            },
            
            InitialHeadingSpeed: GetRandomHeading(50, 125),
            MaxSpawnTime: 2250 * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 7500f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 25,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets reach 1.5x as far!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletLifeTime(1.5f)
                },
                new()
                {
                    SpawnScore = 40,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets travel 1.5x as fast!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletSpeed(1.5f)
                }
             });

        public static LevelData Level2 => new(
            Description: "Level 2",
            WinScore: 250,
            OnEnterSpawn: 2,
            OnEnterSpawnRadius: WindowWidth / 4,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Stone),
            },
            InitialHeadingSpeed: GetRandomHeading(75, 150),
            MaxSpawnTime: 2250 * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseBackIn, 6000f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 45,
                    PickupType = PickUp.Type.Weapon,
                    Description = "Triple Shot Weapon!",
                    OnPickUp = () =>
                    {
                        PrimaryWeapon.ChangeBulletSpeed(.66667f);
                        PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.Triple);
                    }
                },
                new ()
                {
                    SpawnScore = 90,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets do 2x damage!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                },
            });

        public static LevelData Level3 => new(
          Description: "Level 3",
          WinScore: 350,
          OnEnterSpawn: 2,
          OnEnterSpawnRadius: WindowWidth / 4,
          AsteroidSpawnDuring: new()
          {
                (Asteroid.Size.Big, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Stone)
          },
          InitialHeadingSpeed: GetRandomHeading(150, 250),
          MaxSpawnTime: 2000 * SharpRayConfig.TickMultiplier,
          Easing: new(Easings.EaseCircIn, 5500, isRepeated: true),
          PickUps: new()
          {
                new()
                {
                    SpawnScore = 75,
                    PickupType = PickUp.Type.Weapon,
                    Description = "Quintuple Shot Weapon!",
                    OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.Quintuple)
                },
                new()
                {
                    SpawnScore = 175,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets do 3x damage!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Heavy)
                },
          });

        public static LevelData TestLevel => new(
            Description: "Test Level",
            WinScore: 500,
            OnEnterSpawn: 10,
            OnEnterSpawnRadius: 200,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
            },
            InitialHeadingSpeed: new Vector2(1.5f, 1.5f),
            MaxSpawnTime: 1500f * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 5000f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 15,
                    PickupType = PickUp.Type.Weapon,
                    Description = "Triple Shooter Weapon!",
                    OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.Triple),
                },
                new()
                {
                    SpawnScore = 30,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets do 2x Damage!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                }
            });

        public static Vector2 GetRandomHeading(int min, int max) =>
            new(GetRandomValue(min, max) / 100f, GetRandomValue(min, max) / 100f);

    }
}
