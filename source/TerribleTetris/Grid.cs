using System.Diagnostics;
using static TerribleTetris.Game;

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
			Position = new Vector2(( WindowWidth - _data.Width ) / 2, ( WindowHeight - _data.Height ) / 2);
			
			for (var r = 0 ;r < _data.Rows ;r++)
				for (var c = 0 ;c < _data.Cols ;c++)
					Contents.Add((c, r), Shape.None);

		}
		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			DrawDebugIndices( );
		}

		private void DrawDebugIndices()
		{
			foreach (var (c,r) in Contents.Keys)
			{
				var pos = Position + new Vector2(c * _data.CellSize, r * _data.CellSize);
				DrawTextV($"{c}, {r}", pos, 8, BLACK);
			}

			
		}

		public static bool CanMove((int x, int y) idx)
		{
			if (Contents.TryGetValue(idx, out var s))
			{
				return s == Shape.None;
			}

			return false;
		}
	}
}