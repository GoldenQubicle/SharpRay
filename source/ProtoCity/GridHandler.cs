using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;

namespace ProtoCity
{
    public class GridHandler : Entity
    {
        public static int CellSize { get; private set; }
        private static int RowSize { get; set; }

        private static Dictionary<int, (Occupant occupant, int id)> GridCells = new();

        public static int SelectedCellIndex { get; private set; }
        public static Vector2 SelectedCellCenter { get; private set; }
        public static Occupant SelectedCellOccupant { get; private set; }

        public GridHandler(int cellSize)
        {
            CellSize = cellSize;
            RowSize = Program.WindowWidth / CellSize;
        }

        public override void Render()
        {
            DrawTextV($"index:{SelectedCellIndex}", Position - new Vector2(15, 15), 15, Color.RAYWHITE);

            foreach (var (idx, cell) in GridCells)
            {
                var (x, y) = IndexToCoordinates(idx);
                DrawText(GridCells[idx].occupant.ToString(), x, y, 15, Color.RAYWHITE);
            }
        }


        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                Position = mm.Position;
                SelectedCellIndex = CoordinatesToIndex(mm.Position);
                SelectedCellCenter = IndexToCenterCoordinatesV(SelectedCellIndex);
                SelectedCellOccupant = GetCellOccupant(mm.Position);
            }
        }

        internal static void AddOccupant(int idx, Occupant occupant) => GridCells.Add(idx, (occupant, idx));

        private static Occupant GetCellOccupant(Vector2 position) =>
            IsCellOccupied(position) ? GridCells[CoordinatesToIndex(position)].occupant : Occupant.None;

        private static bool IsCellOccupied(Vector2 position) =>
            GridCells.ContainsKey(CoordinatesToIndex(position));

        private static int CoordinatesToIndex(Vector2 pos) =>
            RowSize * ((int)pos.Y / CellSize) + ((int)pos.X / CellSize);

        private static (int x, int y) IndexToCoordinates(int index) =>
            (index % RowSize * CellSize, index / RowSize * CellSize);

        private static Vector2 IndexToCoordinatesV(int index) =>
            new Vector2(index % RowSize * CellSize, index / RowSize * CellSize);

        private static Vector2 IndexToCenterCoordinatesV(int index) =>
            new Vector2((index % RowSize * CellSize) + CellSize / 2, (index / RowSize * CellSize) + CellSize / 2);
    }
}
