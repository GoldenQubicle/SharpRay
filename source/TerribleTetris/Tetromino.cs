namespace TerribleTetris;

internal record TetrominoLocked(Shape Shape, Rotation Rotation, Vector2 BbIndex) : IGameEvent;

internal class Tetromino : Entity, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>
{
	private readonly Shape _shape;
	public Action<IGameEvent> EmitEvent { get; set; }
	private Rotation _rotation;
	private Vector2 _bbIndex;
	private readonly Easing _dropTimer;
	private readonly Easing _moveTimer;
	private readonly Vector2 _bbSize;
	private readonly Vector2 _cellSize;
	private readonly List<(Vector2, Vector2)> _debugIndices = new( );
	private bool _isActive = true;
	private double _dropTimerMultiplier = 1;
	private Vector2 _movement;

	public Tetromino(Shape shape, Rotation rotation, int startCol, int cellSize)
	{
		_shape = shape;
		_rotation = rotation;

		_cellSize = new Vector2(cellSize, cellSize);
		_bbSize = _cellSize * BoundingBoxSize(shape);
		_bbIndex = new Vector2(startCol, 0 - GetOffsets( ).Min(o => o.Y));
		_dropTimer = new Easing(Easings.EaseExpoInOut, DropTime, isRepeated: true);
		_moveTimer = new Easing(Easings.EaseExpoInOut, DropTime/4, isRepeated: true);

	}

	public override void Render()
	{
		

		foreach (var offset in GetOffsets( ))
		{
			var pos = OffsetToScreen(_bbIndex, offset);
			DrawRectangleV(pos, _cellSize, Color(_shape));
			//DrawCircleV(pos, 3, YELLOW);
		}

		Position = BbIdxToScreen(_bbIndex);
		DrawRectangleLinesV(Position, _bbSize, BLUE);
		//DrawDebugOffsetIndices( );
	}

	private void DrawDebugOffsetIndices()
	{
		_debugIndices.Clear( );
		foreach (var offset in GetOffsets( ))
		{
			var pos = OffsetToScreen(_bbIndex, offset);
			var v = OffsetToGridIdx(_bbIndex, offset);
			_debugIndices.Add((pos, v));
		}

		_debugIndices.ForEach(t => DrawTextV($"{t.Item2.X}, {t.Item2.Y}", t.Item1, 8, BLACK));
	}



	public override void Update(double deltaTime)
	{
		if (!_isActive)
			return;

		_moveTimer.Update(deltaTime);
		_dropTimer.Update(deltaTime * _dropTimerMultiplier);

		if (_moveTimer.IsDone())
		{
			_bbIndex += _movement;
		}

		if (_dropTimer.IsDone())
		{
			if (CanMoveDown( ))
				_bbIndex += Vector2.UnitY;
			else
			{
				EmitEvent(new TetrominoLocked(_shape, _rotation, _bbIndex));
				_isActive = false;
				//RemoveEntity(this);
			}
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

		_movement = e switch
		{
			KeyRightDown when CanMoveRight( ) => MoveRight( ),
			KeyLeftDown when CanMoveLeft( ) => MoveLeft( ),
			_ => Vector2.Zero
		};

		_dropTimerMultiplier = e switch
		{
			KeyDownDown => 8.0,
			KeyDownReleased => 1.0,
			_ => _dropTimerMultiplier
		};
	}

	private bool CanRotateClockwise() =>
		CanRotate(RotateClockwise( ));

	private bool CanRotateCounterClockwise() =>
		CanRotate(RotateCounterClockwise( ));

	private bool CanRotate(Rotation rotation) =>
		GetEntity<Grid>( ).CanMove(GetOffsets(rotation), _bbIndex);

	private Rotation RotateClockwise() =>
		(Rotation)( (int)( _rotation + 1 ) % 4 );

	private Rotation RotateCounterClockwise() =>
		(Rotation)( _rotation == 0 ? 3 : (int)_rotation - 1 );

	private bool CanMoveDown() =>
		GetEntity<Grid>( ).CanMove(GetOffsets( ), _bbIndex + Vector2.UnitY);

	private bool CanMoveLeft() =>
		GetEntity<Grid>( ).CanMove(GetOffsets( ), _bbIndex + MoveLeft( ));

	private bool CanMoveRight() =>
		GetEntity<Grid>( ).CanMove(GetOffsets( ), _bbIndex + MoveRight( ));

	private Vector2 MoveLeft() => -Vector2.UnitX;

	private Vector2 MoveRight() => Vector2.UnitX;

	private List<Vector2> GetOffsets(Rotation rotation) =>
		GetOffsets(_shape, rotation);

	private List<Vector2> GetOffsets() =>
		GetOffsets(_shape, _rotation);

	public static int BoundingBoxSize(Shape shape) => shape switch
	{
		Shape.I => 4,
		Shape.O => 2,
		_ => 3
	};

	public static Color Color(Shape shape) => shape switch
	{
		Shape.I => SKYBLUE,
		Shape.O => YELLOW,
		Shape.T => PURPLE,
		Shape.J => DARKBLUE,
		Shape.L => ORANGE,
		Shape.S => LIME,
		Shape.Z => RED,
		Shape.None => BLANK,
		_ => throw new ArgumentOutOfRangeException(nameof(Shape), shape, null)
	};

	private static IDictionary<Shape, Dictionary<Rotation, List<Vector2>>> Offsets =>
		new Dictionary<Shape, Dictionary<Rotation, List<Vector2>>>
		{
				{
					Shape.I, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 1), new(1, 1), new(2, 1), new(3, 1) } },
						{ Rotation.Right, new() { new(2, 0), new(2, 1), new(2, 2), new(2, 3) } },
						{ Rotation.Down, new() { new(0, 2), new(1, 2), new(2, 2), new(3, 2) } },
						{ Rotation.Left, new() { new(1, 0), new(1, 1), new(1, 2), new(1, 3) } },
					}
				},
				{
					Shape.O, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Right, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Down, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
					}
				},
				{
					Shape.T, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(1, 0), new(0, 1), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Left, new() { new(1, 0), new(0, 1), new(1, 1), new(1, 2) } }
					}
				},
				{
					Shape.J, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(0, 1), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(2, 0), new(1, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(2, 1), new(2, 2) } },
						{ Rotation.Left, new() { new(0, 2), new(1, 0), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.L, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 1), new(1, 1), new(2, 1), new(2, 0) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(1, 2), new(2, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(0, 2), new(1, 1), new(2, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(1, 0), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.S, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(1, 0), new(2, 0), new(0, 1), new(1, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(2, 1), new(2, 2) } },
						{ Rotation.Down, new() { new(0, 2), new(1, 1), new(1, 2), new(2, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(0, 1), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.Z, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(1, 0), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(2, 0), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(1, 2), new(2, 2) } },
						{ Rotation.Left, new() { new(0, 1), new(0, 2), new(1, 0), new(1, 1) } },
					}
				},
				{
					Shape.None, new Dictionary<Rotation, List<Vector2>>()

				},
		};

	public static List<Vector2> GetOffsets(Shape shape, Rotation rotation) =>
		Offsets[shape][rotation];

}
