using SharpRay.Entities;
using SharpRay.Eventing;
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


    public struct ShipHitAsteroid : IGameEvent
    {

    }

    public struct BulletLifeTimeExpired : IGameEvent
    {
        public Bullet Bullet { get; init; }
    }
}
