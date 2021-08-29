using System.Numerics;
using static SharpRay.Core.Application;
using SharpRay.Core;
using SharpRay.Eventing;
using System;
using Raylib_cs;

namespace Asteroids
{
    public static class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);

            AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), new Vector2(64, 64)), OnGameEvent);

            AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(65, 100), new Vector2(.5f, 0), 2), OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(65, 100), new Vector2(-.5f, 0), 2), OnGameEvent);

            Run(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipFiredBullet sfb)
            {
                AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), OnGameEvent);
            }

            if (e is BulletLifeTimeExpired ble)
                RemoveEntity(ble.Bullet);

            if (e is BulletHitAsteroid bha)
                RemoveEntity(bha.Bullet);

            if (e is ShipHitAsteroid sha)
                Console.WriteLine("Ship has taken damage");

            if (e is AsteroidDestroyed ad)
                RemoveEntity(ad.Asteroid);

            if (e is AsteroidSpawnNew asn)
            {
                var heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau), MathF.Sin(MathF.Tau));
                AddEntity(new Asteroid(asn.Position, asn.Size, heading, asn.Stage), OnGameEvent);

                heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau * .33333f), MathF.Sin(MathF.Tau * .33333f));
                AddEntity(new Asteroid(asn.Position, asn.Size, heading, asn.Stage), OnGameEvent);

                heading = asn.Heading + new Vector2(MathF.Cos(MathF.Tau * .66666f), MathF.Sin(MathF.Tau * .66666f));
                AddEntity(new Asteroid(asn.Position, asn.Size, heading, asn.Stage), OnGameEvent);
            }
        }

        public static void OnGuiEvent(IGuiEvent e)
        {
        }

        public static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed kp && kp.KeyboardKey == KeyboardKey.KEY_E)
            {
                RemoveEntitiesOfType<Asteroid>();
                AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(65, 100), new Vector2(.5f, 0), 2), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(65, 100), new Vector2(-.5f, 0), 2), OnGameEvent);
            }
        }

    }
}
