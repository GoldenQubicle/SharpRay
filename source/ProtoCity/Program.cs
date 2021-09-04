using Raylib_cs;
using SharpRay.Core;
using SharpRay.Eventing;
using SharpRay.Gui;
using System.Linq.Expressions;
using System.Numerics;
using static SharpRay.Core.Application;


namespace ProtoCity
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AddEntity(new Zone());
            //AddEntity(new Circle
            //{
            //    Position = new Vector2(100, 100),
            //    Radius = 15,
            //    ColorDefault = Color.RED,
            //    ColorFocused = Color.PINK
            //});
            SetKeyBoardEventAction(OnKeyBoardEvent);
            Run(new Config { WindowWidth = 1080, WindowHeight = 720 });
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
