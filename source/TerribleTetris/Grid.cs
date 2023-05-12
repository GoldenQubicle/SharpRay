namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private readonly GridData _data;
		private readonly Texture2D _texture;
		private readonly Dictionary<(int x, int y), Shape> _contents = new();

		public Grid(GridData grid)
		{
			_data = grid;

			Position = _data.Position;
			
			for (var r = 0 ;r < _data.Rows ;r++)
				for (var c = 0 ;c < _data.Cols ;c++)
					_contents.Add((c, r), Shape.None);

			_texture = GetTexture2D("grid");

		}
		

		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			DrawDebugContents( );

			DrawDebugIndices( );

		}

		private void DrawDebugContents()
		{
			foreach (var (idx, s) in _contents)
			{
				var pos = Position + new Vector2(idx.x * _data.CellSize, idx.y * _data.CellSize);
				var data = new TetrominoData(s);
				DrawRectangleV(pos, new Vector2(_data.CellSize, _data.CellSize), data.Color);
			}
		}

		private void DrawDebugIndices()
		{
			foreach (var (c,r) in _contents.Keys)
			{
				var pos = Position + new Vector2(c * _data.CellSize, r * _data.CellSize);
				DrawTextV($"{c}, {r}", pos, 8, RED);
			}
		}

		public void LockCells(TetrominoLocked tb) =>
			tb.Offsets.ForEach(o => _contents[TetrominoOffsetToGridIndices(o, tb.BbIndex)] = tb.Shape);

		public bool CanMove(List<(int x, int y)> offsets, Vector2 toCheck) =>
			offsets.All(o => CanMove(o, toCheck));

		public  bool CanMove((int x, int y) offset, Vector2 toCheck)
		{
			var idx = TetrominoOffsetToGridIndices(offset, toCheck);

			if (_contents.TryGetValue(idx, out var s))
			{
				return s == Shape.None;
			}

			return false;
		}
	}
}