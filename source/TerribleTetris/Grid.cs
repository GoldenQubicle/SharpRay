using static TerribleTetris.Game;

namespace TerribleTetris
{
	internal class Grid : Entity, IHasRender
	{
		private readonly GridData _data;
		private readonly Texture2D _texture;
		public Grid(GridData grid)
		{
			_data = grid;
			var bgImage = GenImageChecked(_data.Width, _data.Height, _data.CellSize, _data.CellSize, MAROON, DARKPURPLE);
			_texture = LoadTextureFromImage(bgImage);
			UnloadImage(bgImage);
			Position = new Vector2(( WindowWidth - _data.Width ) / 2, ( WindowHeight - _data.Height ) / 2);
		}
		public override void Render()
		{
			DrawTextureV(_texture, Position, WHITE);

			for (int r = 0 ;r < _data.Rows ;r++)
			{
				for (int c = 0 ;c < _data.Cols ;c++)
				{
					var pos = Position + new Vector2(c * _data.CellSize, r * _data.CellSize);
					DrawTextV($"{r}, {c}", pos, 15, WHITE);
				}
			}
		}
	}
}