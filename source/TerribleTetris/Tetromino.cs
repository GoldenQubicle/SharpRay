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
			CanMoveDown( );

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

			if (_easing.IsDone( ) && CanMoveDown( ))
			{
				_mapY = (Position.Y, Position.Y + _grid.CellSize);
			}

			Position = new Vector2(_x, MapRange(_easing.GetValue( ), 0f, 1f, _mapY.start, _mapY.end));
		}


		/// <summary>
		/// Given the position of the bounding box, and an absolute offset within it (e.g. [1,2]),
		/// calculate the corresponding grid index.  
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="bbPos"></param>
		/// <returns></returns>
		private (int x, int y) OffsetToGridIndices((int x, int y) offset, Vector2 bbPos)
		{
			// The position in screen pixel coordinates.
			var offsetPosition = bbPos + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
			// The position in absolute pixel coordinates
			var absPosition = offsetPosition - _grid.Position;
			// Divide the absolute pixels by the cell size, also in pixels, to arrive at the grid index. 
			return ((int)absPosition.X / _grid.CellSize, (int)absPosition.Y / _grid.CellSize);
		}

		
		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			_rotation = e switch
			{
				KeyUpReleased or KeyPressed { KeyboardKey: KeyboardKey.KEY_X } when CanRotateClockwise( ) => RotateClockwise( ),
				KeyPressed { KeyboardKey: KeyboardKey.KEY_LEFT_CONTROL } or
				KeyPressed { KeyboardKey: KeyboardKey.KEY_RIGHT_CONTROL } or
				KeyPressed { KeyboardKey: KeyboardKey.KEY_Z } when CanRotateCounterClockwise( ) => RotateCounterClockwise( ),
				_ => _rotation
			};

			_x = e switch
			{
				KeyRightReleased when CanMoveRight( ) => MoveRight( ),
				KeyLeftReleased when CanMoveLeft( ) => MoveLeft( ),
				_ => _x
			};

			Print((Rotation)_x);
		}
	
		private int RotateClockwise() => ( _rotation + 1 ) % 4;

		private int RotateCounterClockwise() => _rotation == 0 ? 3 : _rotation - 1;

		private bool CanRotateClockwise() => CanRotate((Rotation)RotateClockwise( ));

		private bool CanRotateCounterClockwise() => CanRotate((Rotation)RotateCounterClockwise( ));

		private bool CanRotate(Rotation rotation) => _shape.Offsets[rotation]
			.All(o => Grid.CanMove(OffsetToGridIndices(o, new Vector2(_x, _mapY.end)))); // _mapY.end prevented it from rotating on the floor, however, I suspect this will come back to haunt me. 

		private bool CanMoveDown()
		{
			_debugIndices.Clear( );
			var anchor = new Vector2(_x, _mapY.end);
			foreach (var offset in _shape.Offsets[(Rotation)_rotation])
			{
				var pos = Position + new Vector2(offset.x * _grid.CellSize, offset.y * _grid.CellSize);
				var (x, y) = OffsetToGridIndices(offset, anchor);
				_debugIndices.Add((pos, x, y));
			}

			var b = _shape.Offsets[(Rotation)_rotation]
				.All(o => Grid.CanMove(OffsetToGridIndices(o, anchor + new Vector2(0, _grid.CellSize))));

			return b;
		}

		private float MoveLeft() => _x - _grid.CellSize;

		private float MoveRight() => _x + _grid.CellSize;

		private bool CanMoveLeft() => Grid.CanMove(OffsetToGridIndices(_shape.Offsets[(Rotation)_rotation].MinBy(o => o.x),
			new Vector2(MoveLeft( ), _mapY.start)));

		private bool CanMoveRight() => Grid.CanMove(OffsetToGridIndices(_shape.Offsets[(Rotation)_rotation].MaxBy(o => o.x),
			new Vector2(MoveRight( ), _mapY.start)));

	}
}