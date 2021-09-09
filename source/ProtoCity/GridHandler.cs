using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using System.ComponentModel.DataAnnotations;

namespace ProtoCity
{
    public class GridHandler : Entity
    {
        public static int CellSize { get; private set; }
        private static int RowSize { get; set; }

        private static Dictionary<int, (Occupant occupant, int id)> GridCells = new();

        private int SelectedCellIndex;
        private Vector2 SelectedCellCenter;

        public GridHandler(int cellSize)
        {
            CellSize = cellSize;
            RowSize = Program.WindowWidth / CellSize;
        }

        public override void Render()
        {
            var center = IndexToCenterCoordinatesV(SelectedCellIndex);
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
        }


        internal static void AddOccupant(int idx, Occupant occupant) => GridCells.Add(idx, (occupant, idx));

        internal static (int idx, Occupant occupant) GetCellInfo(Vector2 position) =>
            (CoordinatesToIndex(position), GetCellOccupant(position));

        internal static Occupant GetCellOccupant(Vector2 position) =>
            IsCellOccupied(position) ? GridCells[CoordinatesToIndex(position)].occupant : Occupant.None;

        internal static bool IsCellOccupied(Vector2 position) =>
            GridCells.ContainsKey(CoordinatesToIndex(position));

        internal static int CoordinatesToIndex(Vector2 pos) =>
            RowSize * ((int)pos.Y / CellSize) + ((int)pos.X / CellSize);

        internal static (int x, int y) IndexToCoordinates(int index) =>
            (index % RowSize * CellSize, index / RowSize * CellSize);

        internal static Vector2 IndexToCoordinatesV(int index) =>
            new Vector2(index % RowSize * CellSize, index / RowSize * CellSize);

        internal static Vector2 IndexToCenterCoordinatesV(int index) =>
            new Vector2((index % RowSize * CellSize) + CellSize / 2, (index / RowSize * CellSize) + CellSize / 2);
    }
}
