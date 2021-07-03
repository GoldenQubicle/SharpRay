using SharpRay.Entities;
using SharpRay.Eventing;
using System.Dynamic;
using System.Numerics;

namespace Asteroids
{
    public struct ShipFiredBullet : IGameEvent
    {
        public Vector2 Origin { get; init; }
        public float Angle { get; init; }
        public float Force { get; init; }
    }

    public struct BulletHitAsteroid : IGameEvent
    {
        public Bullet Bullet { get; init; }

    }

    public struct AsteroidDestroyed : IGameEvent
    {
        public Asteroid Asteroid { get; init; }
    }

    public struct AsteroidSpawnNew : IGameEvent
    {
        public int Stage { get; init; }
        public Vector2 Position { get; init; }
        public Vector2 Size { get; init; }
        public Vector2 Heading { get; init; }
        public float Rotation { get; init; }
    }

    public struct ShipHitAsteroid : IGameEvent
    {
        public int DamageDone { get; init; }
    }

    public struct BulletLifeTimeExpired : IGameEvent
    {
        public Bullet Bullet { get; init; }
    }
}
