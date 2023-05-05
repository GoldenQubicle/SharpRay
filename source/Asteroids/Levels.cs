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
            OnEnterSpawn: 3,
            OnEnterSpawnRadius: 540,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
            },
            
            InitialHeadingSpeed: GetRandomHeading(50, 125),
            MaxSpawnTime: 2000 * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 4500f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 35,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets reach 1.5x as far!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletLifeTime(1.5f)
                },
                new()
                {
                    SpawnScore = 60,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets travel 1.5x as fast!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletSpeed(1.5f)
                }
             });

        public static LevelData Level2 => new(
            Description: "Level 2",
            WinScore: 250,
            OnEnterSpawn: 4,
            OnEnterSpawnRadius: 480,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Big, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Stone),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
            },
            InitialHeadingSpeed: GetRandomHeading(75, 150),
            MaxSpawnTime: 2000 * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseBackIn, 5000f, isRepeated: true),
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
                    SpawnScore = 110,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets do 2x damage!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                },
            });

        public static LevelData Level3 => new(
          Description: "Level 3",
          WinScore: 500,
          OnEnterSpawn: 5,
          OnEnterSpawnRadius: 360,
          AsteroidSpawnDuring: new()
          {
                (Asteroid.Size.Big, Asteroid.Type.Dirt),
                (Asteroid.Size.Big, Asteroid.Type.Stone),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
          },
          InitialHeadingSpeed: GetRandomHeading(125, 200),
          MaxSpawnTime: 2000 * SharpRayConfig.TickMultiplier,
          Easing: new(Easings.EaseCircIn, 5500, isRepeated: true),
          PickUps: new()
          {
                new()
                {
                    SpawnScore = 125,
                    PickupType = PickUp.Type.Weapon,
                    Description = "Quintuple Shot Weapon!",
                    OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.Quintuple)
                },
                new()
                {
                    SpawnScore = 225,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets do 3x damage!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Heavy)
                },
          });

        public static LevelData TestLevel => new(
            Description: "Test Level",
            WinScore: 500,
            OnEnterSpawn: 1,
            OnEnterSpawnRadius: 200,
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Big, Asteroid.Type.Stone),
                //(Asteroid.Size.Small, Asteroid.Type.Dirt),
                //(Asteroid.Size.Medium, Asteroid.Type.Stone),
            },
            InitialHeadingSpeed: new Vector2(1.5f, 1.5f),
            MaxSpawnTime: 150000f * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 5000f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 10,
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
