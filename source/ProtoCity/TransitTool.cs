namespace ProtoCity
{
    public class TransitTool : GuiEntity
    {
        public bool IsActive { get; set; }
        private Dictionary<int, TransitNode> Nodes = new();
        private int prevIdx = -1;

        public override void Render()
        {
            var sCenter = GridHandler.GetSelectedCenter();

            if (IsActive)
            {
                DrawRectangleLinesV(sCenter - GridHandler.CellSizeV / 2, GridHandler.CellSizeV, Color.Beige);
            }

            foreach (var node in Nodes.Values)
            {
                //DrawCircleV(node.Position, 3, Color.DARKPURPLE);
                DrawTextV(node.Connections.Count.ToString(), node.Position, 10, Color.RayWhite);

                foreach (var con in node.Connections)
                {
                    DrawLineV(node.Position, con.Position, Color.Purple);
                }
            }

            if (prevIdx != -1)
            {
                //DrawCircleV(sCenter, 3, Color.PINK);
                DrawLineV(Nodes[prevIdx].Position, sCenter, Color.Lime);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (!IsActive) return;

            var (idx, occupant, center) = GridHandler.GetSelected();

            if (occupant is Occupant.Zone) return;

            if (e is MouseLeftClick mlc && !mlc.IsHandled)
            {
                if (occupant is Occupant.None)
                {
                    GridHandler.AddOccupant(idx, Occupant.TransitNode);
                    Nodes.Add(idx, new TransitNode(idx, center));
                }

                if (prevIdx != -1)
                {
                    Nodes[idx].Connections.Add(Nodes[prevIdx]);
                    Nodes[prevIdx].Connections.Add(Nodes[idx]);
                }

                prevIdx = idx;
            }

            if (e is MouseRightClick mrc && prevIdx != -1)
            {
                if (Nodes[prevIdx].Connections.Count == 0)
                {
                    Nodes.Remove(prevIdx);
                    GridHandler.RemoveCell(prevIdx);
                }

                prevIdx = -1;
            }
        }

        public record TransitNode(int Idx, Vector2 Position)
        {
            public List<TransitNode> Connections { get; } = new();
        }

        internal void Clear()
        {
            prevIdx = -1;
            Nodes.Clear();
        }
    }
}
