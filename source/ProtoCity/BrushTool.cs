namespace ProtoCity
{
    public class BrushTool : GuiEntity
    {
        public bool IsActive { get; set; }

        private float radius = (GridHandler.CellSize / 2) + (GridHandler.CellSize * 2);
        private float radiusIncrement = GridHandler.CellSize;
        private float radiusMin = GridHandler.CellSize / 2;
        private float radiusMax = GridHandler.CellSize * 20; // ideally this somehow scales w decrementing cell size

        public override void Render()
        {
            if (IsActive)
            {
                var sCenter = GridHandler.GetSelectedCenter();
                DrawCircleLinesV(sCenter, radius, Color.Beige);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (!IsActive) return;

            if (e is MouseLeftDrag mlc)
            {
                var sCenter = GridHandler.GetSelectedCenter();

                var tl = new Vector2(sCenter.X - radius, sCenter.Y - radius);
                var s = new Vector2(radius * 2, radius * 2);

                foreach (var (idx, occupant, center) in GridHandler.GetCellsInRect(tl, s))
                {
                    if (occupant == Occupant.None && CheckCollisionPointCircle(center, sCenter, radius))
                    {
                        GridHandler.AddOccupant(idx, Occupant.Zone);
                    }
                }
            }

            if (e is MouseWheelUp mwu && radius < radiusMax)
                radius += radiusIncrement;

            if (e is MouseWheelDown mwd && radius > radiusMin)
                radius -= radiusIncrement;
        }

        internal void Clear()
        {

        }
    }
}
