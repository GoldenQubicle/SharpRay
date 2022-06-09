﻿namespace Asteroids
{
    internal class Levels
    {
        public static List<LevelData> Data => new()
        {
           Level1, Level2, TestLevel
        };

        public static LevelData Level1 => new(
            Description: "Level 1",
            WinScore: 50,
            ShipLayout: new(
                Position: new(WindowWidth / 2, WindowHeight / 2),
                Health: MaxHealth),
            Lifes: 3,
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
            InitialHeadingSpeed: GetRandomHeading(50, 150),
            MaxSpawnTime: 2250 * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 7500f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    SpawnScore = 20,
                    Description = "Bullets reach 1.5x as far!",
                    OnPickUp = () => PrimaryWeapon.ChangeBulletLifeTime(1.5f)
                },
             });
        public static LevelData Level2 => new(
    Description: "Level 2",
    WinScore: 125,
    ShipLayout: new(
        Position: new(WindowWidth / 2, WindowHeight / 2),
        Health: MaxHealth),
    Lifes: 3,
    AsteroidSpawnStart: new()
    {
                   new (Asteroid.Size.Big, Asteroid.Type.Dirt, new (GetRandomValue(WindowWidth-256, WindowWidth-128), GetRandomValue(WindowHeight-256, WindowHeight-128)), GetRandomHeading(-75, 75)),
    },
    AsteroidSpawnDuring: new()
    {
                   (Asteroid.Size.Large, Asteroid.Type.Dirt),
                   (Asteroid.Size.Medium, Asteroid.Type.Stone),
                   (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                   (Asteroid.Size.Small, Asteroid.Type.Stone),
    },
    InitialHeadingSpeed: GetRandomHeading(100, 200),
    MaxSpawnTime: 2250 * SharpRayConfig.TickMultiplier,
    Easing: new(Easings.EaseBackInOut, 6000f, isRepeated: true),
    PickUps: new()
    {
                   new ()
                   {
                       SpawnScore = 25,
                       Description = "Bullets do 2x damage!",
                       OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                   },
                   new ()
                   {
                       SpawnScore = 45,
                       Description = "Triple Shot Weapon!",
                       OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.TripleNarrow)
                   },
     });

        public static LevelData TestLevel => new(
            Description: "Test Level",
            WinScore: 100,
            ShipLayout: new(
                Position: new(WindowWidth / 2, WindowHeight / 2),
                Health: MaxHealth),
            Lifes: 2,
            AsteroidSpawnStart: new()
            {
                new (Asteroid.Size.Large, Asteroid.Type.Dirt, new (800, 100), new (0, 1.5f)),
            },
            AsteroidSpawnDuring: new()
            {
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
                (Asteroid.Size.Small, Asteroid.Type.Dirt),
                (Asteroid.Size.Medium, Asteroid.Type.Dirt),
            },
            InitialHeadingSpeed: new Vector2(1.5f, 1.5f),
            MaxSpawnTime: 1500f * SharpRayConfig.TickMultiplier,
            Easing: new(Easings.EaseSineInOut, 5000f, isRepeated: true),
            PickUps: new()
            {
                new ()
                {
                    Description = "Triple Shooter Weapon!",
                    SpawnScore = 15,
                    OnPickUp = () => PrimaryWeapon.ChangeMode(PrimaryWeapon.Mode.TripleNarrow)
                },
                new()
                {
                    Description = "Bullets do 2x Damage!",
                    SpawnScore = 30,
                    OnPickUp = () => PrimaryWeapon.ChangeBulletType(Bullet.Type.Medium)
                }
            });

        public static Vector2 GetRandomHeading(int min, int max) =>
            new(GetRandomValue(min, max) / 100f, GetRandomValue(min, max) / 100f);

    }
}
