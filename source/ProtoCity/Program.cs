using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Gui;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace ProtoCity
{
    public static class Program
    {
        public static int WindowWidth = 1080;
        public static int WindowHeight = 720;
        public static int CellSize = 40;

        public static void Main(string[] args)
        {
            Initialize(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });
            
            var image = GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, Color.BEIGE, Color.BROWN);
            AddEntity(new ImageTexture(image, Color.GRAY));
            AddEntity(new GridHandler(CellSize));

            SetKeyBoardEventAction(OnKeyBoardEvent);
            SetMouseEventAction(OnMouseEvent);

            Run();
        }

        private static void OnMouseEvent(IMouseEvent e)
        {
            
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            
        }
    }
}
