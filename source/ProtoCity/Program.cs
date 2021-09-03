using SharpRay.Core;
using SharpRay.Eventing;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AddEntity(new Zone());
            SetKeyBoardEventAction(OnKeyBoardEvent);
            Run(new Config { WindowWidth = 1080, WindowHeight = 720 });
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent e) 
        {
            if(e is KeySpaceBarPressed)
            {
                RemoveEntitiesOfType<Zone>();
                AddEntity(new Zone());
            }
        }
    }
}
