using static TerribleTetris.Game;

namespace TerribleTetris;

internal static partial class Game
{
	public record TetrominoLocked(Shape Shape, Rotation Rotation, Vector2 BbIndex) : IGameEvent;

	internal class Tetromino : Entity, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>
	{
		private readonly Shape _shape;
		public Action<IGameEvent> EmitEvent { get; set; }
		private Rotation _rotation;
		private Vector2 _bbIndex;
		private readonly Easing _dropTimer;
		private readonly Vector2 _bbSize;
		private readonly List<(Vector2, Vector2)> _debugIndices = new( );
		private bool _isActive = true;

		public Tetromino(Shape shape, int startCol)
		{
			_shape = shape;
			_bbIndex = new Vector2(startCol, 0);
			Position = IndexToScreen(_bbIndex);
			_bbSize = new(TetrominoData.BoundingBoxSize(shape) * GridData.CellSize, TetrominoData.BoundingBoxSize(shape) * GridData.CellSize);
			_dropTimer = new Easing(Easings.EaseExpoInOut, DropTime, isRepeated: true);
		}


		public override void Render()
		{
			Position = IndexToScreen(_bbIndex);

			//DrawRectangleLinesV(Position, _bbSize, BLUE);

			foreach (var offset in TetrominoData.GetOffsets(_shape, _rotation))
			{
				var pos = Position + new Vector2(offset.X * GridData.CellSize, offset.Y * GridData.CellSize);
				DrawRectangleV(pos, new Vector2(GridData.CellSize, GridData.CellSize), TetrominoData.Color(_shape));
				//DrawCircleV(pos, 3, YELLOW);
			}

			//DrawDebugOffsetIndices();
		}

		public override void Update(double deltaTime)
		{
			if (!_isActive)
				return;

			_dropTimer.Update(deltaTime);

			if (!_dropTimer.IsDone( ))
				return;

			if (CanMoveDown( ))
				_bbIndex += Vector2.UnitY;
			else
			{
				EmitEvent(new TetrominoLocked(_shape, _rotation, _bbIndex));
				_isActive = false;
				//RemoveEntity(this);
			}
		}

		public override void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (!_isActive)
				return;

			_rotation = e switch
			{
				KeyUpReleased or KeyPressed { KeyboardKey: KeyboardKey.KEY_X } when CanRotateClockwise( ) => RotateClockwise( ),
				KeyPressed { KeyboardKey: KeyboardKey.KEY_LEFT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_RIGHT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_Z } when CanRotateCounterClockwise( ) => RotateCounterClockwise( ),
				_ => _rotation
			};

			_bbIndex = e switch
			{
				KeyRightReleased when CanMoveRight( ) => MoveRight( ),
				KeyLeftReleased when CanMoveLeft( ) => MoveLeft( ),
				_ => _bbIndex
			};
		}

		private bool CanRotateClockwise() =>
			CanRotate(RotateClockwise( ));

		private bool CanRotateCounterClockwise() =>
			CanRotate(RotateCounterClockwise( ));

		private bool CanRotate(Rotation rotation) =>
			GetEntity<Grid>().CanMove(GetRotationOffsets(rotation), _bbIndex);

		private Rotation RotateClockwise() =>
			(Rotation)( (int)( _rotation + 1 ) % 4 );

		private Rotation RotateCounterClockwise() =>
			(Rotation)( _rotation == 0 ? 3 : (int)_rotation - 1 );

		private bool CanMoveDown() =>
			GetEntity<Grid>( ).CanMove(TetrominoData.GetOffsets(_shape,_rotation), _bbIndex + Vector2.UnitY);

		private void DrawDebugOffsetIndices()
		{
			_debugIndices.Clear();
			foreach (var offset in TetrominoData.GetOffsets(_shape, _rotation))
			{
				var pos = OffsetToScreen(_bbIndex, offset);
				var v = TetrominoOffsetToGridIndices(offset, _bbIndex);
				_debugIndices.Add((pos, v));
			}

			_debugIndices.ForEach(t => DrawTextV($"{t.Item2.X}, {t.Item2.Y}", t.Item1, 8, BLACK));
		}

		private bool CanMoveLeft() =>
			GetEntity<Grid>( ).CanMove(TetrominoData.GetOffsets(_shape, _rotation), MoveLeft( ));

		private bool CanMoveRight() =>
			GetEntity<Grid>( ).CanMove(TetrominoData.GetOffsets(_shape, _rotation), MoveRight( ));

		private Vector2 MoveLeft() =>
			_bbIndex - Vector2.UnitX;

		private Vector2 MoveRight() =>
			_bbIndex + Vector2.UnitX;

		private List<Vector2> GetRotationOffsets(Rotation rotation) =>
			TetrominoData.GetOffsets(_shape, rotation);
		
	}
}