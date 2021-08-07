using System.Numerics;
using static SharpRay.Core.Application;
using SharpRay.Eventing;
using System;
using SharpRay.Collision;

namespace Asteroids
{
    public class Game
    {
        public static int WindowWidth = 1080;
        public static int WindowHeight = 720;

        static void Main(string[] args)
        {
            AddEntity(new Ship
            {
                Size = new Vector2(512, 512),
                Position = new Vector2(0, 0),
                Collider = new RectCollider
                {
                    Size = new Vector2(512, 512),
                    Position = new Vector2(0, 0),
                }
            });

            Run(args);

        }


        static void OnGuiEvent(IGuiEvent e)
        {
            Console.WriteLine($"hello gui event {e}");
        }
    }
}
