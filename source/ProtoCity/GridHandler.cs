using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using SharpRay.Gui;
using System.Runtime.Intrinsics.X86;

namespace ProtoCity
{
    public class GridHandler : Entity
    {
        public static int SelectedCellIndex { get; private set; }
        private static int CellSize { get; set; }
        private static int RowSize { get; set; }

        private Dictionary<int, (Occupant occupant, int id)> GridCells = new();

        public GridHandler(int cellSize)
        {
            CellSize = cellSize;
            RowSize = Program.WindowWidth / CellSize;
        }

        public override void Render()
        {
            DrawTextV($"index:{SelectedCellIndex}", Position - new Vector2(15, 15), 15, Color.RAYWHITE);

            foreach (var cell in GridCells.Keys)
            {
                var (x, y) = IndexToCoordinates(cell);
                DrawText(GridCells[cell].occupant.ToString(), x, y, 15, Color.RAYWHITE);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                Position = mm.Position;
                SelectedCellIndex = CoordinatesToIndex(mm.Position);
            }

            if (e is MouseLeftClick mlc)
            {
                if (GridCells.ContainsKey(SelectedCellIndex))
                {

                }
                else
                {
                    var id = 0;
                    GridCells.Add(SelectedCellIndex, (Occupant.StreetNode, id));
                    var edge = new Edge(
                        id,
                        new PointHandler
                        {
                            Position = IndexToCoordinatesV(SelectedCellIndex),
                            Radius = CellSize / 2,
                            ColorDefault = Color.DARKBLUE,
                            ColorFocused = Color.BLUE,
                        },
                        new PointHandler
                        {
                            Position = IndexToCoordinatesV(SelectedCellIndex),
                            Radius = CellSize / 2,
                            ColorDefault = Color.DARKBLUE,
                            ColorFocused = Color.BLUE,
                            IsSelected = true
                        });
                    AddEntity(edge);
                }
            }
        }

        public static int CoordinatesToIndex(Vector2 pos) => RowSize * ((int)pos.Y / CellSize) + ((int)pos.X / CellSize);

        public static (int x, int y) IndexToCoordinates(int index) => (index % RowSize * CellSize, index / RowSize * CellSize);
        public static Vector2 IndexToCoordinatesV(int index) => new Vector2(index % RowSize * CellSize, index / RowSize * CellSize);

    }
}
