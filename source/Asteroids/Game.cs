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

            Run(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

        }

        static void OnGameEvent(IGameEvent e)
        {
            if (e is ShipShootBullet ss)
                AddEntity(new Bullet(ss.Origin, ss.Angle, ss.Force), OnGameEvent);

            if(e is BulletLifeTimeExpired b)
                RemoveEntity(b.Bullet);

        }

        static void OnGuiEvent(IGuiEvent e)
        {
        }
    }
}
