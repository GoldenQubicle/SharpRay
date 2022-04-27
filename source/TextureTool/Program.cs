using SharpRay.Eventing;
using SharpRay.Core;
using Raylib_cs;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;

namespace TextureTool
{
    class Program
    {
        private const int size = 512;
        private const string ExportFolder = @"..\..\..\export";
        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);

            Initialize(new SharpRayConfig { WindowHeight = size, WindowWidth = size });
            
            AddEntity(new Star(size));

            Run();
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if(ke is KeyPressed kp && kp.KeyboardKey is KeyboardKey.KEY_S)
            {
                var image = GetScreenData();
                
                ExportImage(image, $"{ExportFolder}\\star_v1.png");
            }
        }
    }
}
