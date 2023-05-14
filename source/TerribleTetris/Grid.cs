namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private readonly GridData _data;
		private readonly Texture2D _texture;
		private Dictionary<Vector2, Shape> Cells { get; } = new();

		public Grid(GridData grid)
		{
			_data = grid;

			Position = _data.Position;
			
			for (var r = 0 ;r < _data.Rows ;r++)
				for (var c = 0 ;c < _data.Cols ;c++)
					Cells.Add(new (c, r), Shape.None);

			_texture = GetTexture2D("grid");
			
		}
		

		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			//DrawDebugCells( );

			//DrawDebugIndices( );

		}

		private void DrawDebugCells()
		{
			foreach (var (idx, s) in Cells)
			{
				var pos = Position + new Vector2(idx.X * _data.CellSize, idx.Y * _data.CellSize);
				DrawRectangleV(pos, new Vector2(_data.CellSize, _data.CellSize), Tetromino.Color(s));
			}
		}

		private void DrawDebugIndices()
		{
			foreach (var v in Cells.Keys)
			{
				var pos = Position + new Vector2(v.X * _data.CellSize, v.Y * _data.CellSize);
				DrawTextV($"{v.X}, {v.Y}", pos, 8, RED);
			}
		}

		public void LockCells(TetrominoLocked tb) =>
			Tetromino.GetOffsets(tb.Shape, tb.Rotation)
				.Select(o => OffsetToGridIdx(o, tb.BbIndex)).ToList()
				.ForEach(o => Cells[o] = tb.Shape);

		public bool CanMove(List<Vector2> offsets, Vector2 bbIdx) =>
			offsets.All(o => CanMove(o, bbIdx));

		public  bool CanMove(Vector2 bbIdx, Vector2 offset)
		{
			var idx = OffsetToGridIdx(bbIdx, offset);

			if (Cells.TryGetValue(idx, out var s))
			{
				return s == Shape.None;
			}

			return false;
		}
	}
}