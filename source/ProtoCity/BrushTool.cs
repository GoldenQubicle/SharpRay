using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class BrushTool : Entity
    {
        private float radius = GridHandler.CellSize / 2;
        private float radiusIncrement = GridHandler.CellSize /4;
        private float radiusMin = GridHandler.CellSize /2;
        private float radiusMax = GridHandler.CellSize * 10;
        public override void Render()
        {
            DrawCircleLinesV(GridHandler.SelectedCellCenter, radius, Color.BEIGE);
        }

        public override void OnMouseEvent(IMouseEvent e)
        {

            if(e is MouseLeftClick mlc)
            {
                
            }

            if (e is MouseWheelUp mwu && radius < radiusMax)
                radius += radiusIncrement;

            if (e is MouseWheelDown mwd && radius > radiusMin)
                radius -= radiusIncrement;
        }

    }
}
