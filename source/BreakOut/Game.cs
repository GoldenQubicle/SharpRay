global using System;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using Raylib_cs;
global using static Raylib_cs.Raylib;
global using SharpRay.Core;
global using SharpRay.Components;
global using SharpRay.Collision;
global using SharpRay.Eventing;
global using SharpRay.Entities;
global using SharpRay.Interfaces;
global using SharpRay.Gui;
global using SharpRay.Listeners;
global using static SharpRay.Core.Application;
global using static SharpRay.Core.Audio;
global using static BreakOut.Game;

namespace BreakOut
{
    public static class Game
    {
        internal const int WindowWidth = 720;
        internal const int WindowHeight = 480;
       

        public static void Main()
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                ShowFPS = true,
                DoEventLogging = true,
            });

            AddEntity(new Paddle());
            AddEntity(new Ball());
            PlaceBricks();
            Run();
        }

        public static void PlaceBricks()
        {
            var columns = 5;
            var rows = 3;
            var marginX = 20;
            var marginY = 40;
            var width = 100;
            var height = 20;
            for(var c = 0; c < columns; c++)
            {
                for(var r = 0; r < rows; r++)
                {
                    var top = new Vector2(c * (width + marginX) + 60, r * (height + marginY) + 50);
                    var bottom = top + new Vector2(0, WindowHeight/2);
                    AddEntity(new Brick(top));
                    AddEntity(new Brick(bottom));
                }
            };
        }
    }
}


