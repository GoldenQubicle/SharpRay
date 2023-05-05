using Rectangle = Raylib_cs.Rectangle;

namespace TerribleTetris;

internal static partial class Game
{
	internal class Tetromino : Entity, IHasRender, IHasUpdate
	{
		private readonly TetrominoData _shape;
		private readonly GridData _grid;
		private Rectangle _boundingBox;
		private int _rotation;

		public Tetromino(TetrominoData shape, GridData grid)
		{
			_shape = shape;
			_grid = grid;
			var bbSize = _shape.BoundingBoxSize * _grid.CellSize;
			Position = grid.Position;
			_boundingBox = new Rectangle(Position.X, Position.Y, bbSize, bbSize);
		}

		public override void Render()
		{
			DrawRectangleLinesEx(_boundingBox, 3, BLUE);
			foreach (var offset in _shape.Offsets[(Rotation)_rotation])
			{
				var pos = Position + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
				DrawRectangleV(pos, new Vector2(_grid.CellSize, _grid.CellSize), _shape.Color);
			}
		}

		public override void Update(double deltaTime)
		{

		}

		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			_rotation = e switch
			{
				KeyLeftReleased => _rotation == 0 ? 3 : _rotation - 1,
				KeyRightReleased => ( _rotation + 1 ) % 4,
				_ => _rotation
			};
		}
	}
}