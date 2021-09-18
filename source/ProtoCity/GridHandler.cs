using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using System;
using System.Diagnostics;

namespace ProtoCity
{
    public class GridHandler : Entity
    {
        public static Vector2 CellSizeV { get; private set; }
        public static int CellSize { get; private set; }
        private static int RowSize { get; set; }

        private static Dictionary<int, GridCell> GridCells = new();

        private static int SelectedCellIndex { get; set; }
        private static Vector2 SelectedCellCenter { get; set; }
        private static Occupant SelectedCellOccupant { get; set; }

        public GridHandler(int cellSize)
        {
            CellSize = cellSize;
            RowSize = Program.WindowWidth / CellSize;
            CellSizeV = new Vector2(CellSize, CellSize);
        }

        public override void Render()
        {
            //DrawTextV($"index:{SelectedCellIndex}", Position - new Vector2(15, 15), 15, Color.RAYWHITE);

            foreach (var (idx, cell) in GridCells)
            {
                var (x, y) = IndexToCoordinates(idx);
                //DrawText(GridCells[idx].occupant.ToString(), x, y, 15, Color.RAYWHITE);

                if (cell.Occupant == Occupant.Zone)
                {
                    DrawRectangleV(IndexToCoordinatesV(idx), CellSizeV, Color.BEIGE);
                }

                if(cell.Occupant == Occupant.TransitNode)
                {
                    DrawCircleV(cell.Center, 3, Color.DARKPURPLE);
                }
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

        internal static GridCell GetSelected() => new(SelectedCellIndex, SelectedCellOccupant, SelectedCellCenter);

        internal static Vector2 GetSelectedCenter() => SelectedCellCenter;

        internal static IEnumerable<GridCell> GetCellsInRect(Vector2 topLeft, Vector2 size)
        {
            var cols = size.X / CellSize;
            var rows = size.Y / CellSize;

            for (var x = 0; x < cols; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    var rp = topLeft + new Vector2(x * CellSize, y * CellSize);
                    var idx = CoordinatesToIndex(rp);
                    yield return new GridCell(idx, GetCellOccupant(idx), IndexToCenterCoordinatesV(idx));
                }
            }
        }

        internal static void RemoveCell(int idx) => 
            GridCells.Remove(idx);

        internal static void AddOccupant(int idx, Occupant occupant) =>
            GridCells.Add(idx, new(idx, occupant, IndexToCenterCoordinatesV(idx)));

        private static Occupant GetCellOccupant(int idx) =>
            IsCellOccupied(idx) ? GridCells[idx].Occupant : Occupant.None;

        private static bool IsCellOccupied(int idx) =>
            GridCells.ContainsKey(idx);

        private static Occupant GetCellOccupant(Vector2 position) =>
            IsCellOccupied(position) ? GridCells[CoordinatesToIndex(position)].Occupant : Occupant.None;

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

        public record GridCell(int Idx, Occupant Occupant, Vector2 Center);
    }
}
