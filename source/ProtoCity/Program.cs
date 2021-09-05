using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Gui;
using System.Diagnostics;
using System.Numerics;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public static class Program
    {
        public static int WindowWidth = 1080;
        public static int WindowHeight = 720;

        public static void Main(string[] args)
        {
            Initialize(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

            AddEntity(new Agent
            {
                Position = new Vector2(WindowWidth/2, WindowHeight/2)
            });
            //AddEntity(new Zone());
            SetKeyBoardEventAction(OnKeyBoardEvent);

            Run();
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeySpaceBarPressed)
            {
                RemoveEntitiesOfType<Zone>();
                RemoveEntitiesOfType<PointHandler>();
                AddEntity(new Zone());
            }
        }
    }
}
