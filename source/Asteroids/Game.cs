using System.Numerics;
using static SharpRay.Core.Application;
using SharpRay.Core;
using SharpRay.Eventing;
using System.Collections.Generic;
using System;
using Raylib_cs;
using SharpRay.Entities;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Game : Entity
    {
        public const int WindowWidth = 1080;
        public const int WindowHeight = 720;

        private static Dictionary<Type, int> bulletStats = new()
        {
            {typeof(ShipFiredBullet), 0 },
            {typeof(BulletHitAsteroid), 0 }
        };

        static void Main(string[] args)
        {
            var game = new Game();
            
            AddEntity(game);

            AddEntity(new Ship(new Vector2(WindowWidth / 2, WindowHeight / 2), new Vector2(64, 64)), game.OnGameEvent);

            AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(65, 100), new Vector2(.5f,0), 15, 2), game.OnGameEvent);
            AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(65, 100), new Vector2(-.5f,0), 15, 2), game.OnGameEvent);

            Run(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

        }

        public override void Render()
        {
            //DrawText("hi there!", WindowWidth / 2, WindowHeight / 2, 38, Color.ORANGE);
        }

        public void OnGameEvent(IGameEvent e)
        {
            if (e is ShipFiredBullet sfb)
            {
                AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), OnGameEvent);
                bulletStats[sfb.GetType()]++;
            }

            if (e is BulletLifeTimeExpired ble)
                RemoveEntity(ble.Bullet);

            if (e is BulletHitAsteroid bha)
            {
                RemoveEntity(bha.Bullet);
                bulletStats[bha.GetType()]++;
            }

            if(e is ShipHitAsteroid sha)
                Console.WriteLine("Ship has taken damage");

            if (e is AsteroidDestroyed ad)
            {
                //TODO increase score
                RemoveEntity(ad.Asteroid);
            }

            if(e is AsteroidSpawnNew asn)
            {
                //AddEntity(new Asteroid(asn.SpawnPoint, asn.Size, 10, asn.Stages), OnGameEvent);
                //AddEntity(new Asteroid(asn.SpawnPoint + new Vector2(asn.Size.X, 0), asn.Size, 10, asn.Stages), OnGameEvent);
                //AddEntity(new Asteroid(asn.SpawnPoint + new Vector2(0, asn.Size.Y), asn.Size, 10, asn.Stages), OnGameEvent);
                //AddEntity(new Asteroid(asn.SpawnPoint + new Vector2(asn.Size.X, asn.Size.Y), asn.Size, 10, asn.Stages), OnGameEvent);
            }

        }

        public void OnGuiEvent(IGuiEvent e)
        {
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if(e is KeyPressed kp && kp.KeyboardKey == KeyboardKey.KEY_E) 
            {
                RemoveEntities<Asteroid>();
                AddEntity(new Asteroid(new Vector2(150, 100), new Vector2(65, 100), new Vector2(.5f, 0), 15, 2), OnGameEvent);
                AddEntity(new Asteroid(new Vector2(350, 100), new Vector2(65, 100), new Vector2(-.5f, 0), 15, 2), OnGameEvent);
            }
        }
    }
}
