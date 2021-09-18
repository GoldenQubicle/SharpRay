using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class BrushTool : Entity
    {
        private float radius = GridHandler.CellSize / 2;
        private float radiusIncrement = GridHandler.CellSize / 2;
        private float radiusMin = GridHandler.CellSize / 2;
        private float radiusMax = GridHandler.CellSize * 10;

        public override void Render()
        {
            var sCenter = GridHandler.GetSelectedCenter();

            DrawCircleLinesV(sCenter, radius, Color.BEIGE);

            var tl = new Vector2(sCenter.X - radius, sCenter.Y - radius);
            var s = new Vector2(radius * 2, radius * 2);

            DrawRectangleLinesV(tl, s, Color.GREEN);

            foreach (var cell in GridHandler.GetCellsInRect(tl, s))
            {
                if (CheckCollisionPointCircle(cell.Center, sCenter, radius))
                {
                    var size = new Vector2(GridHandler.CellSize, GridHandler.CellSize);
                    DrawRectangleV(cell.Center - size / 2, size, Color.LIME);
                }
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {

            if (e is MouseMovement mm)
            {
                //get cells within bounding box, check if cell center is within brush circle..?

            }

            if (e is MouseLeftClick mlc)
            {

            }

            if (e is MouseWheelUp mwu && radius < radiusMax)
                radius += radiusIncrement;

            if (e is MouseWheelDown mwd && radius > radiusMin)
                radius -= radiusIncrement;
        }

    }
}
