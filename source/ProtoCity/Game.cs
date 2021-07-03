using SharpRay.Gui;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using SharpRay.Eventing;
using System;
using SharpRay.Collision;
using Raylib_cs;

namespace ProtoCity
{
    public class Game
    {
        public static int WindowWidth = 1080;
        public static int WindowHeight = 720;

        static void Main(string[] args)
        {
            AddEntity(new World
            {
                Size = new Vector2(512, 512),
                Position = new Vector2(0, 0),
                Origin = new Vector2(256, 256),
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
