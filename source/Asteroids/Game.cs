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
            AddEntity(new Ship(new Vector2(128, 128), new Vector2(WindowWidth/2, WindowHeight/2)));

            Run(args);

        }


        static void OnGuiEvent(IGuiEvent e)
        {
            Console.WriteLine($"hello gui event {e}");
        }
    }
}
