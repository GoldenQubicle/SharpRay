using Rectangle = Raylib_cs.Rectangle;

namespace TerribleTetris;

internal static partial class Game
{
	internal class Tetromino : Entity, IHasRender, IHasUpdate
	{
		private readonly TetrominoData _shape;
		private readonly GridData _grid;
		private int _rotation;
		private Easing _easing;
		private float _nextY;
		private float _oldY;
		private float _x;

		private Vector2 _bbSize;

		public Tetromino(TetrominoData shape, GridData grid)
		{
			_shape = shape;
			_grid = grid;
			_bbSize = new(_shape.BoundingBoxSize * _grid.CellSize, _shape.BoundingBoxSize * _grid.CellSize);
			Position = grid.Position;
			_easing = new Easing(Easings.EaseBounceOut, LevelTimer);
			_oldY = Position.Y;
			_nextY = Position.Y + grid.CellSize;
			_x = Position.X;
		}

		public override void Render()
		{
			DrawRectangleLinesV(Position, _bbSize, BLUE);
			foreach (var offset in _shape.Offsets[(Rotation)_rotation])
			{
				var pos = Position + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
				DrawRectangleV(pos, new Vector2(_grid.CellSize, _grid.CellSize), _shape.Color);
			}
		}

		public override void Update(double deltaTime)
		{
			_easing.Update(deltaTime);

			if (_easing.IsDone( ))
			{
				_oldY = Position.Y;
				_nextY = Position.Y + _grid.CellSize;
				_easing.Reset( );
				return;
			}

			Position = new Vector2(_x, MapRange(_easing.GetValue( ), 0f, 1f, _oldY, _nextY));

		}

		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			_rotation = e switch
			{
				KeyUpReleased or KeyPressed { KeyboardKey: KeyboardKey.KEY_X } => ( _rotation + 1 ) % 4,
				KeyPressed { KeyboardKey: KeyboardKey.KEY_LEFT_CONTROL } or
				KeyPressed { KeyboardKey: KeyboardKey.KEY_RIGHT_CONTROL } or
				KeyPressed { KeyboardKey: KeyboardKey.KEY_Z } => _rotation == 0 ? 3 : _rotation - 1,
				_ => _rotation
			};

			_x = e switch
			{
				KeyRightReleased => _x + _grid.CellSize,
				KeyLeftReleased => _x - _grid.CellSize,
				_ => _x
			};
		}
	}
}