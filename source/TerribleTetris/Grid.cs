namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private static  GridData _data;
		private readonly Texture2D _texture;
		private static readonly Dictionary<(int x, int y), Shape> Contents = new();

		public Grid(GridData grid)
		{
			_data = grid;
			var bgImage = GenImageChecked(_data.Width, _data.Height, _data.CellSize, _data.CellSize, _data.Color1, _data.Color2);
			_texture = LoadTextureFromImage(bgImage);
			UnloadImage(bgImage);
			Position = _data.Position;
			
			for (var r = 0 ;r < _data.Rows ;r++)
				for (var c = 0 ;c < _data.Cols ;c++)
					Contents.Add((c, r), Shape.None);

		}
		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			DrawDebugContents( );

			DrawDebugIndices( );

		}

		private static void DrawDebugContents()
		{
			foreach (var (idx, s) in Contents)
			{
				var pos = Game.GridData.Position + new Vector2(idx.x * Game.GridData.CellSize, idx.y * Game.GridData.CellSize);
				var data = new TetrominoData(s);
				DrawRectangleV(pos, new Vector2(Game.GridData.CellSize, Game.GridData.CellSize), data.Color);
			}
		}

		private void DrawDebugIndices()
		{
			foreach (var (c,r) in Contents.Keys)
			{
				var pos = Position + new Vector2(c * _data.CellSize, r * _data.CellSize);
				DrawTextV($"{c}, {r}", pos, 8, RED);
			}
		}

		public static void BlockCells(TetrominoBlocked tb) => 
			tb.Indices.ForEach(o => Contents[o] = tb.Shape);

		public static bool CanMove(List<(int x, int y)> offsets, Vector2 toCheck) =>
			offsets.All(o => CanMove(o, toCheck));

		public static bool CanMove((int x, int y) offset, Vector2 toCheck)
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