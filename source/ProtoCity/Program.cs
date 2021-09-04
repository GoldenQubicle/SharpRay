using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Gui;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Initialize(new Config { WindowWidth = 1080, WindowHeight = 720, DoEventLogging = true });

            AddEntity(new Zone());
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
