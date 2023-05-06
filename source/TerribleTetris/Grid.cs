using System.Diagnostics;
using static TerribleTetris.Game;

namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private readonly GridData _data;
		private readonly Texture2D _texture;
		private static Dictionary<(int x, int y), Shape> _contents = new();

		public Grid(GridData grid)
		{
			_data = grid;
			var bgImage = GenImageChecked(_data.Width, _data.Height, _data.CellSize, _data.CellSize, _data.Color1, _data.Color2);
			_texture = LoadTextureFromImage(bgImage);
			UnloadImage(bgImage);
			Position = new Vector2(( WindowWidth - _data.Width ) / 2, ( WindowHeight - _data.Height ) / 2);
			
			for (int r = 0 ;r < _data.Rows ;r++)
				for (int c = 0 ;c < _data.Cols ;c++)
					_contents.Add((c, r), Shape.None);

		}
		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			//DrawDebugIndices( );
		}

		private void DrawDebugIndices()
		{
			for (int r = 0; r < _data.Rows; r++)
			{
				for (int c = 0; c < _data.Cols; c++)
				{
					var pos = Position + new Vector2(c * _data.CellSize, r * _data.CellSize);
					DrawTextV($"{c}, {r}", pos, 8, BLACK);
				}
			}
		}

		public static bool CanMove((int x, int y) idx)
		{
			if (_contents.TryGetValue(idx, out Shape s))
			{
				return s == Shape.None;
			}

			return false;
		}

	}
}