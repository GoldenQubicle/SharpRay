using SharpRay.Core;
using SharpRay.Gui;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace ProtoCity
{
    public static class Program
    {
        internal static int WindowWidth = 1080;
        internal static int WindowHeight = 720;
        internal static int CellSize = 40;

        public static void Main(string[] args)
        {
            Initialize(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });
            
            var background = GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, Color.BEIGE, Color.BROWN);
            AddEntity(new ImageTexture(background, Color.GRAY));
            AddEntity(new GridHandler(CellSize));

            

            Run();
        }
    }
}
