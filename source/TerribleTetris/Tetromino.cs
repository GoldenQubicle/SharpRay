namespace TerribleTetris;

internal static partial class Game
{
	public record TetrominoBlocked(Shape Shape, List<(int, int)> Indices) : IGameEvent;

	internal class Tetromino : Entity, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>
	{
		public Action<IGameEvent> EmitEvent { get; set; }
		public Rotation Rotation { get; set; }
		//public int X { get; set; }
		//public int Y { get; set; }
		public Vector2 Index { get; set; }

		private readonly TetrominoData _data;
		private readonly Easing _dropTimer;
		private readonly Vector2 _bbSize;
		//private (float start, float end) _mapY;
		private readonly List<(Vector2, int, int)> _debugIndices = new( );
		private bool _isActive = true;

		public Tetromino(Shape shape, int startCol)
		{
			Index = new Vector2(startCol, 0);
			Position = IndexToScreen();
			_data = new TetrominoData(shape);
			_bbSize = new(_data.BoundingBoxSize * GridData.CellSize, _data.BoundingBoxSize * GridData.CellSize);
			_dropTimer = new Easing(Easings.EaseExpoInOut, DropTime, isRepeated: true);
			//_mapY = (Position.Y, Position.Y + GridData.CellSize);
			//X = Position.X;
			//CanMoveDown( );
		}

		private Vector2 IndexToScreen() => GridData.Position + (Index * GridData.CellSize);

		public override void Render()
		{
			Position = IndexToScreen();

			DrawRectangleLinesV(Position, _bbSize, BLUE);

			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = Position + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				DrawRectangleV(pos, new Vector2(GridData.CellSize, GridData.CellSize), _data.Color);
				//DrawCircleV(pos, 3, YELLOW);
			}

			_debugIndices.ForEach(t => DrawTextV($"{t.Item2}, {t.Item3}", t.Item1, 8, BLACK));
		}

		public override void Update(double deltaTime)
		{
			if (!_isActive)
				return;

			//if (!CanMoveDown( ) && _isActive)
			//{
			//	EmitEvent(new TetrominoBlocked(_data.Shape, _data.Offsets[Rotation]
			//		.Select(o => TetrominoOffsetToGridIndices(o, new Vector2(X, _mapY.end))).ToList( )));
			//	_isActive = false;
			//	RemoveEntity(this);
			//	return;
			//}

			_dropTimer.Update(deltaTime);

			if(!_dropTimer.IsDone()) return;

			if (CanMoveDown())
			{
				Index += Vector2.UnitY;
			}
			
			//if (_dropTimer.IsDone( ) && CanMoveDown( ))
			//{
			//	_mapY = (Position.Y, Position.Y + GridData.CellSize);
			//}

			//Position = new Vector2(X, MapRange(_dropTimer.GetValue( ), 0f, 1f, _mapY.start, _mapY.end));
		}

		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (!_isActive) return;

			Rotation = e switch
			{
				KeyUpReleased or KeyPressed { KeyboardKey: KeyboardKey.KEY_X } when CanRotateClockwise( ) => RotateClockwise( ),
				KeyPressed { KeyboardKey: KeyboardKey.KEY_LEFT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_RIGHT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_Z } when CanRotateCounterClockwise( ) => RotateCounterClockwise( ),
				_ => Rotation
			};

			Index = e switch
			{
				KeyRightReleased when CanMoveRight( ) => MoveRight( ),
				KeyLeftReleased when CanMoveLeft( ) => MoveLeft( ),
				_ => Index
			};

			
		}

		private  Rotation RotateClockwise() =>
			(Rotation)( (int)( Rotation + 1 ) % 4 );
		private  Rotation RotateCounterClockwise() =>
			(Rotation)( Rotation == 0 ? 3 : (int)Rotation - 1 );

		private  bool CanRotateClockwise() =>
			CanRotate(RotateClockwise( ));

		private  bool CanRotateCounterClockwise() =>
			CanRotate(RotateCounterClockwise( ));

		private  bool CanRotate(Rotation rotation) =>
			Grid.CanMove(GetRotationOffsets(rotation), Index);

		private  Vector2 MoveLeft() =>
			Index - Vector2.UnitX;

		private  Vector2 MoveRight() =>
			Index + Vector2.UnitX;

		private  bool CanMoveLeft() =>
			Grid.CanMove(GetLeftMostX( ), MoveLeft() );

		private  bool CanMoveRight() =>
			Grid.CanMove(GetRightMostX( ), MoveRight());
		private bool CanMoveDown()
		{
			_debugIndices.Clear( );
			var anchor = Index + Vector2.UnitY;
			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = Position + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				var (x, y) = TetrominoOffsetToGridIndices(offset, anchor);
				_debugIndices.Add((pos, x, y));
			}

			return Grid.CanMove(_data.Offsets[Rotation], anchor);
		}

		public List<(int x, int y)> GetLeftMostX() =>
			_data.Offsets[Rotation]
				.GroupBy(o => o.x)
				.OrderBy(g => g.Key)
				.First( )
				.ToList( );

		public List<(int x, int y)> GetRightMostX() =>
			_data.Offsets[Rotation]
				.GroupBy(o => o.x)
				.OrderBy(g => g.Key)
				.Last( )
				.ToList( );

		public List<(int x, int y)> GetRotationOffsets(Rotation rotation) =>
			_data.Offsets[rotation];
	}
}