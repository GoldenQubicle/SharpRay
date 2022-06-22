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
            WinScore: 50,
            AsteroidSpawnStart: new()
            {
                new (Asteroid.Size.Large, Asteroid.Type.Dirt, new (GetRandomValue(128, 256), GetRandomValue(128, 256)), GetRandomHeading(-50, 50)),
            },
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
                    SpawnScore = 20,
                    PickupType = PickUp.Type.Bullet,
                    Description = "Bullets reach 1.5x as far!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletLifeTime(1.5f)
                },
             });

        public static LevelData Level2 => new(
            Description: "Level 2",
            WinScore: 125,
            AsteroidSpawnStart: new()
            {
                new (Asteroid.Size.Big, Asteroid.Type.Dirt,
                    new (GetRandomValue(WindowWidth-256, WindowWidth-128), GetRandomValue(WindowHeight-256, WindowHeight-128)),GetRandomHeading(-75, 75)),
            },
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
                       SpawnScore = 25,
                       PickupType = PickUp.Type.Weapon,
                       Description = "Triple Shot Weapon!",
                       OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.TripleNarrow)
                   },
                   new ()
                   {
                       SpawnScore = 45,
                       PickupType = PickUp.Type.Bullet,
                       Description = "Bullets do 2x damage!",
                       OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                   },
            });

        public static LevelData Level3 => new(
          Description: "Level 3",
          WinScore: 350,
          AsteroidSpawnStart: new()
          {
                new (Asteroid.Size.Big, Asteroid.Type.Stone,
                    new (GetRandomValue(WindowWidth/3, WindowWidth/4), GetRandomValue(WindowHeight/3, WindowHeight/4)),
                    GetRandomHeading(15, 50)),
          },
          AsteroidSpawnDuring: new()
          {
                (Asteroid.Size.Big, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Stone),
                (Asteroid.Size.Large, Asteroid.Type.Dirt),
                (Asteroid.Size.Large, Asteroid.Type.Stone)
          },
          InitialHeadingSpeed: GetRandomHeading(150, 250),
          MaxSpawnTime: 1500 * SharpRayConfig.TickMultiplier,
          Easing: new(Easings.EaseCircIn, 5000, isRepeated: true),
          PickUps: new()
          {
                new(){
                           SpawnScore = 75,
                           PickupType = PickUp.Type.Weapon,
                           Description = "Quintuple Shot Weapon!",
                           OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.Quintuple)
                      },
                new(){
                           SpawnScore = 175,
                           PickupType = PickUp.Type.Bullet,
                           Description = "Bullets do 3x damage!",
                           OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Heavy)
                     },
          });

        public static LevelData TestLevel => new(
            Description: "Test Level",
            WinScore: 500,
            AsteroidSpawnStart: new()
            {
                new (Asteroid.Size.Large, Asteroid.Type.Dirt, new (800, 100), new (0, 1.5f)),
            },
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
                    OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.TripleNarrow),
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
