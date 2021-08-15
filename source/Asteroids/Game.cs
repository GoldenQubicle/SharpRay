using System.Numerics;
using static SharpRay.Core.Application;
using SharpRay.Core;
using SharpRay.Eventing;

namespace Asteroids
{
    public class Game
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        static void Main(string[] args)
        {
            AddEntity(new Ship(new Vector2(64, 64), new Vector2(WindowWidth / 2, WindowHeight / 2)), OnGameEvent);

            AddEntity(new Asteroid(new Vector2(100, 100), new Vector2(50, 20)), OnGameEvent);

            Run(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

        }

        static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipFiredBullet sfb)
                AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), OnGameEvent);

            if(e is BulletLifeTimeExpired b)
                RemoveEntity(b.Bullet);

            if (e is BulletHitAsteroid bh)
                RemoveEntity(bh.Bullet);

            if(e is ShipHitAsteroid sha)
                System.Console.WriteLine("Ship has taken damage");

        }

        static void OnGuiEvent(IGuiEvent e)
        {
        }
    }
}
