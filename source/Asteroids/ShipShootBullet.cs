using SharpRay.Entities;
using SharpRay.Eventing;
using System.Numerics;

namespace Asteroids
{
    public struct ShipShootBullet : IGameEvent
    {
        public Vector2 Origin { get; init; }
        public float Rotation { get; init; }
        public float Force { get; init; }
    }

    public struct BulletLifeTimeExpired : IGameEvent
    {
        public Bullet Bullet { get; init; }
    }
}
