namespace Asteroids
{
    public struct ShipEngineVolume : IGameEvent
    {
        public float NormalizedVolume { get; set; }
    }

    public struct ShipFiredBullet : IGameEvent
    {
        public Vector2 Origin { get; init; }
        public float Angle { get; init; }
        public float Force { get; init; }
    }

    public struct AsteroidHitByWeapon : IGameEvent
    {
        public Asteroid Asteroid { get; init; }
        public Bullet Bullet { get; init; }
    }

    public struct ShipHitAsteroid : IGameEvent
    {
        public Asteroid Asteroid { get; init; }
    }

    public struct BulletLifeTimeExpired : IGameEvent
    {
        public Bullet Bullet { get; init; }
    }
}
