using SharpRay.Eventing;
using SharpRay.Core;
using Raylib_cs;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;

namespace TextureTool
{
    class Program
    {
        private const int size = 64;
        private const string ExportFolder = @"..\..\..\export";
        static void Main(string[] args)
        {
            SetKeyBoardEventAction(OnKeyBoardEvent);

            Initialize(new SharpRayConfig
            {
                WindowHeight = size,
                WindowWidth = size,
                BackGroundColor = Color.BLACK
            });

            AddEntity(new Star(size));
            
            Run();
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (ke is KeyPressed kp && kp.KeyboardKey is KeyboardKey.KEY_S)
            {
                var image = GetScreenData();
                //ImageAlphaClear(ref image, Color.WHITE, .5f);
                ExportImage(image, $"{ExportFolder}\\star_extra_small.png");
            }
        }
    }
}
