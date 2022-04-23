﻿using System.Numerics;
using static SharpRay.Core.Application;
using SharpRay.Core;
using SharpRay.Eventing;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.IO;

namespace Asteroids
{
    public static class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);
            Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

            AddSound(Ship.EngineSound, "spaceEngineLow_001.ogg");
            AddSound(Ship.ThrusterSound, "thrusterFire_001.ogg");

            var shipTexture = LoadTexture(Path.Combine(AssestsFolder, @"PNG\playerShip2_orange.png"));
            var meteorTexture = LoadTexture(Path.Combine(AssestsFolder, @"PNG\Meteors\meteorBrown_big1.png"));

            AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), new Vector2(64, 64), shipTexture), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(.5f, 0), 2, meteorTexture), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 2, meteorTexture), OnGameEvent);

            Run();
        }

        public static void OnGameEvent(IGameEvent e)
        {

            if (e is ShipHitAsteroid sha)
            {
                Console.WriteLine("Ship has taken damage");
            }

            if (e is ShipFiredBullet sfb)
                AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), OnGameEvent);

            if (e is BulletLifeTimeExpired ble)
                RemoveEntity(ble.Bullet);

            if (e is BulletHitAsteroid bha)
                RemoveEntity(bha.Bullet);


            if (e is AsteroidDestroyed ad)
                RemoveEntity(ad.Asteroid);

            if (e is AsteroidSpawnNew asn)
            {
                var meteorTexture = LoadTexture(Path.Combine(AssestsFolder, @"PNG\Meteors\meteorBrown_med1.png"));
                //take heading of parent asteroid into account because it offers more dynamic 'explosion'
                //than just using a 'clean' heading, i.e. Vector(.3, .3)
                var heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau), MathF.Sin(MathF.Tau));
                AddEntity(new Asteroid(asn.Position, heading, asn.Stage, meteorTexture), OnGameEvent);

                heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau * .33333f), MathF.Sin(MathF.Tau * .33333f));
                AddEntity(new Asteroid(asn.Position, heading, asn.Stage, meteorTexture), OnGameEvent);

                heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau * .66666f), MathF.Sin(MathF.Tau * .66666f));
                AddEntity(new Asteroid(asn.Position, heading, asn.Stage, meteorTexture), OnGameEvent);
            }
        }

        public static void OnGuiEvent(IGuiEvent e)
        {
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed kp && kp.KeyboardKey == KeyboardKey.KEY_E)
            {
                var meteorTexture = LoadTexture(Path.Combine(AssestsFolder, @"PNG\Meteors\meteorBrown_big1.png"));
                RemoveEntitiesOfType<Asteroid>();
                AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(.5f, 0), 2, meteorTexture), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(-.5f, 0), 2, meteorTexture), OnGameEvent);
            }
        }

    }
}
