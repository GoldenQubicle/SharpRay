﻿namespace Asteroids
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

    public struct AsteroidDestroyed : IGameEvent
    {
        public Asteroid Asteroid { get; init; }
    }

    public struct ShipLifeLost : IGameEvent
    {
        public int LifeIconIdx { get; init; }
    }

    public struct ShipHitAsteroid : IGameEvent
    {
        public Asteroid Asteroid { get; init; }
    }

    public struct ShipPickUp : IGameEvent
    {
        public PickUp PickUp { get; init; }
    }
}
