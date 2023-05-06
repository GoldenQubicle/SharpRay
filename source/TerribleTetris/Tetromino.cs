namespace TerribleTetris;

internal static partial class Game
{
	internal class Tetromino : Entity, IHasRender, IHasUpdate
	{
		private readonly TetrominoData _shape;
		private readonly GridData _grid;
		private readonly Easing _easing;
		private readonly Vector2 _bbSize;
		private int _rotation;
		private float _x;
		private (float start, float end) _mapY;
		private readonly List<(Vector2, int, int)> _debugIndices = new( );

		public Tetromino(TetrominoData shape, GridData grid)
		{
			Position = grid.Position;
			_shape = shape;
			_grid = grid;
			_bbSize = new(_shape.BoundingBoxSize * _grid.CellSize, _shape.BoundingBoxSize * _grid.CellSize);
			_easing = new Easing(Easings.EaseExpoInOut, LevelTimer, isRepeated: true);
			_mapY = (Position.Y, Position.Y + _grid.CellSize);
			_x = Position.X;
			DebugTranslateScreenToGrid( );

		}

		public override void Render()
		{
			DrawRectangleLinesV(Position, _bbSize, BLUE);

			foreach (var offset in _shape.Offsets[(Rotation)_rotation])
			{
				var pos = Position + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
				DrawRectangleV(pos, new Vector2(_grid.CellSize, _grid.CellSize), _shape.Color);
				//DrawCircleV(pos, 3, YELLOW);
			}

			_debugIndices.ForEach(t => DrawTextV($"{t.Item2}, {t.Item3}", t.Item1, 8, YELLOW));
		}

		public override void Update(double deltaTime)
		{
			if (!CanMoveDown( ))
				return;

			_easing.Update(deltaTime);

			if (_easing.IsDone( ) && CanMoveDown())
			{
				_mapY = (Position.Y, Position.Y + _grid.CellSize);

				_debugIndices.Clear( );
				//DebugTranslateScreenToGrid( );
			}

			Position = new Vector2(_x, MapRange(_easing.GetValue( ), 0f, 1f, _mapY.start, _mapY.end));
		}

		private bool CanMoveDown()
		{
			//Check for obstacles, other tetrominos or the floor
			var b = _shape.Offsets[(Rotation)_rotation]
				.All(o => Grid.CanMove(OffsetToGridIndices(o, new Vector2(_x, _mapY.end + _grid.CellSize))));

			return b;
		}

		private (int x, int y) OffsetToGridIndices((int x, int y) offset, Vector2 bbPos)
		{
			var offsetPosition = bbPos + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
			var absPosition = offsetPosition - _grid.Position;
			return ((int)absPosition.X / _grid.CellSize, (int)absPosition.Y / _grid.CellSize);
		}

		private void DebugTranslateScreenToGrid()
		{
			foreach (var offset in _shape.Offsets[(Rotation)_rotation])
			{
				var pos = Position + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
				var (x, y) = OffsetToGridIndices(offset, Position);
				_debugIndices.Add((pos, x, y));
			}
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