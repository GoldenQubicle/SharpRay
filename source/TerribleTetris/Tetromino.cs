namespace TerribleTetris;

internal static partial class Game
{
	public record TetrominoLocked(Shape Shape, List<(int x, int y)> Offsets, Vector2 BbIndex) : IGameEvent;

	internal class Tetromino : Entity, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>
	{
		public Action<IGameEvent> EmitEvent { get; set; }
		public Rotation Rotation { get; set; }
		public Vector2 Index { get; set; }

		private readonly TetrominoData _data;
		private readonly Easing _dropTimer;
		private readonly Vector2 _bbSize;
		private readonly List<(Vector2, int, int)> _debugIndices = new( );
		private bool _isActive = true;

		public Tetromino(TetrominoData data, int startCol)
		{
			Index = new Vector2(startCol, 0);
			Position = IndexToScreen( );
			_data = data;
			_bbSize = new(_data.BoundingBoxSize * GridData.CellSize, _data.BoundingBoxSize * GridData.CellSize);
			_dropTimer = new Easing(Easings.EaseExpoInOut, DropTime, isRepeated: true);
		}


		public override void Render()
		{
			Position = IndexToScreen( );

			DrawRectangleLinesV(Position, _bbSize, BLUE);

			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = Position + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				DrawRectangleV(pos, new Vector2(GridData.CellSize, GridData.CellSize), _data.Color);
				//DrawCircleV(pos, 3, YELLOW);
			}

			DrawDebugOffsetIndices();
		}

		public override void Update(double deltaTime)
		{
			if (!_isActive)
				return;

			_dropTimer.Update(deltaTime);

			if (!_dropTimer.IsDone( ))
				return;

			if (CanMoveDown( ))
				Index += Vector2.UnitY;
			else
			{
				EmitEvent(new TetrominoLocked(_data.Shape, _data.Offsets[Rotation], Index));
				_isActive = false;
				RemoveEntity(this);
			}
		}

		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (!_isActive)
				return;

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

		private bool CanRotateClockwise() =>
			CanRotate(RotateClockwise( ));

		private bool CanRotateCounterClockwise() =>
			CanRotate(RotateCounterClockwise( ));

		private bool CanRotate(Rotation rotation) =>
			GetEntity<Grid>().CanMove(GetRotationOffsets(rotation), Index);

		private Rotation RotateClockwise() =>
			(Rotation)( (int)( Rotation + 1 ) % 4 );

		private Rotation RotateCounterClockwise() =>
			(Rotation)( Rotation == 0 ? 3 : (int)Rotation - 1 );

		private bool CanMoveDown() =>
			GetEntity<Grid>( ).CanMove(_data.Offsets[Rotation], Index + Vector2.UnitY);

		private void DrawDebugOffsetIndices()
		{
			_debugIndices.Clear();
			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = IndexToScreen() + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				var (x, y) = TetrominoOffsetToGridIndices(offset, Index);
				_debugIndices.Add((pos, x, y));
			}

			_debugIndices.ForEach(t => DrawTextV($"{t.Item2}, {t.Item3}", t.Item1, 8, BLACK));
		}

		private bool CanMoveLeft() =>
			GetEntity<Grid>( ).CanMove(_data.Offsets[Rotation], MoveLeft( ));

		private bool CanMoveRight() =>
			GetEntity<Grid>( ).CanMove(_data.Offsets[Rotation], MoveRight( ));

		private Vector2 MoveLeft() =>
			Index - Vector2.UnitX;

		private Vector2 MoveRight() =>
			Index + Vector2.UnitX;

		private List<(int x, int y)> GetRotationOffsets(Rotation rotation) =>
			_data.Offsets[rotation];

		private Vector2 IndexToScreen() =>
			GridData.Position + ( Index * GridData.CellSize );
	}
}