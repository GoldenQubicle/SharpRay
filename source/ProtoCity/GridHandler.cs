using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Numerics;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class GridHandler : Entity
    {
        public int CellSize { get; }

        private int x;
        private int y;

        public GridHandler(int cellSize)
        {
            CellSize = cellSize;
        }


        public override void Render()
        {
            DrawTextV($"x:{x} y:{y}", Position-new Vector2(15, 0), 15, Color.RAYWHITE);
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                Position = mm.Position;
                x = (int)mm.Position.X / CellSize;
                y = (int)mm.Position.Y / CellSize;
            }
        }
    }
}
