namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private readonly GridData _data;
		private readonly Texture2D _texture;
		public Dictionary<Vector2, Shape> Contents { get; } = new();

		public Grid(GridData grid)
		{
			_data = grid;

			Position = _data.Position;
			
			for (var r = 0 ;r < _data.Rows ;r++)
				for (var c = 0 ;c < _data.Cols ;c++)
					Contents.Add(new (c, r), Shape.None);

			_texture = GetTexture2D("grid");

		}
		

		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			//DrawDebugContents( );

			//DrawDebugIndices( );

		}

		private void DrawDebugContents()
		{
			foreach (var (idx, s) in Contents)
			{
				var pos = Position + new Vector2(idx.X * _data.CellSize, idx.Y * _data.CellSize);
				DrawRectangleV(pos, new Vector2(_data.CellSize, _data.CellSize), TetrominoData.Color(s));
			}
		}

		private void DrawDebugIndices()
		{
			foreach (var v in Contents.Keys)
			{
				var pos = Position + new Vector2(v.X * _data.CellSize, v.Y * _data.CellSize);
				DrawTextV($"{v.X}, {v.Y}", pos, 8, RED);
			}
		}

		public void LockCells(TetrominoLocked tb) =>
			TetrominoData.GetOffsets(tb.Shape, tb.Rotation)
				.ForEach(o => Contents[TetrominoOffsetToGridIndices(o, tb.BbIndex)] = tb.Shape);

		public bool CanMove(List<Vector2> offsets, Vector2 toCheck) =>
			offsets.All(o => CanMove(o, toCheck));

		public  bool CanMove(Vector2 offset, Vector2 toCheck)
		{
			var idx = TetrominoOffsetToGridIndices(offset, toCheck);

			if (Contents.TryGetValue(idx, out var s))
			{
				return s == Shape.None;
			}

			return false;
		}
	}
}