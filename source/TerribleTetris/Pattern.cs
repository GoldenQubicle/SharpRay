namespace TerribleTetris;

internal class Pattern : Entity, IHasRender, IEventEmitter<IGameEvent>
{
	public Action<IGameEvent> EmitEvent { get; set; }
	private readonly PatternData _data;
	private readonly Vector2 _cellSize;
	private readonly Dictionary<Vector2, Shape> _cells;

	private const int MatchShape = 25;
	private const int MatchCellSame = 10;
	private const int MatchCellDifferent = 5;

	private int _totalScore;
	private int _totalShapes;
	private int _totalCells;


	public Pattern(PatternData data, GridData gridData)
	{
		_data = data;
		_cellSize = new(gridData.CellSize, gridData.CellSize);
		_cells = _data.Shapes
			.SelectMany(s => Tetromino.GetOffsets(s.Shape, s.Rotation)
				.Select(o => (OffsetToScreen(s.BbIndex, o), s.Shape))).ToDictionary(o => o.Item1, _ => _.Shape);

	}


	public override void Render()
	{
		foreach (var cell in _cells)
		{
			DrawRectangleV(cell.Key, _cellSize, BEIGE);
		}

		DrawScore( );
	}

	private void DrawScore()
	{
		var anchor = new Vector2(WindowWidth * .7f, GetEntity<Grid>().Position.Y);
		var size = new Vector2(196, 128);
		var margin = 28;
		var fontSize = 18;
		DrawRectangleV(anchor, size, DARKGREEN);
		DrawRectangleLinesV(anchor, size, GREEN);
		DrawTextV($"Score : {_totalScore}", anchor + new Vector2(margin, margin), fontSize, GOLD);
		DrawTextV($"Shapes : {_totalShapes} / {_data.Shapes.Count}", anchor + new Vector2(margin, margin * 2), fontSize, GOLD);
		DrawTextV($"Cells : {_totalCells} / {_cells.Count}", anchor + new Vector2(margin, margin * 3), fontSize, GOLD);
	}


	public void CalculateScore(TetrominoLocked tl)
	{
		var isMatch = CheckForMatches(tl);
		var overlaps = CheckForOverlaps(tl);

		if (overlaps.TryGetValue(true, out var sameCells))
		{
			_totalScore += sameCells * MatchCellSame;
			_totalCells += sameCells;
		}

		if (overlaps.TryGetValue(false, out var diffCells))
		{
			_totalScore += diffCells * MatchCellDifferent;
			_totalCells += diffCells;
		}

		if (isMatch)
		{
			_totalScore += MatchShape;
			_totalShapes++;
			AddEntity(CreateMatchNotification( ));
		}

	}

	private Dictionary<bool, int> CheckForOverlaps(TetrominoLocked tl) =>
		Tetromino.GetOffsets(tl.Shape, tl.Rotation)
			.Select(o => OffsetToScreen(tl.BbIndex, o))
			.Where(o => _cells.ContainsKey(o))
			.Select(o => _cells[o] == tl.Shape)
			.GroupBy(m => m)
			.ToDictionary(t => t.Key, t => t.Count( ));

	private bool CheckForMatches(TetrominoLocked tl)
	{
		// Due to rotation of I, S & Z we need to check both (up & down) and (left & right) because placement is the same.
		// For O we simple check all rotations
		var checkForMatches = tl.Shape switch
		{
			Shape.I when tl.Rotation == Rotation.Up => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Down, tl.BbIndex - Vector2.UnitY) },
			Shape.I when tl.Rotation == Rotation.Down => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Up, tl.BbIndex + Vector2.UnitY) },
			Shape.I when tl.Rotation == Rotation.Right => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Left, tl.BbIndex - Vector2.UnitX) },
			Shape.I when tl.Rotation == Rotation.Left => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Right, tl.BbIndex + Vector2.UnitX) },
			Shape.S when tl.Rotation == Rotation.Up => new List<TetrominoLocked> { tl, new(Shape.S, Rotation.Down, tl.BbIndex - Vector2.UnitY) },
			Shape.S when tl.Rotation == Rotation.Down => new List<TetrominoLocked> { tl, new(Shape.S, Rotation.Up, tl.BbIndex + Vector2.UnitY) },
			Shape.S when tl.Rotation == Rotation.Right => new List<TetrominoLocked> { tl, new(Shape.S, Rotation.Left, tl.BbIndex + Vector2.UnitX) },
			Shape.S when tl.Rotation == Rotation.Left => new List<TetrominoLocked> { tl, new(Shape.S, Rotation.Right, tl.BbIndex - Vector2.UnitX) },
			Shape.Z when tl.Rotation == Rotation.Up => new List<TetrominoLocked> { tl, new(Shape.Z, Rotation.Down, tl.BbIndex - Vector2.UnitY) },
			Shape.Z when tl.Rotation == Rotation.Down => new List<TetrominoLocked> { tl, new(Shape.Z, Rotation.Up, tl.BbIndex + Vector2.UnitY) },
			Shape.Z when tl.Rotation == Rotation.Right => new List<TetrominoLocked> { tl, new(Shape.Z, Rotation.Left, tl.BbIndex + Vector2.UnitX) },
			Shape.Z when tl.Rotation == Rotation.Left => new List<TetrominoLocked> { tl, new(Shape.Z, Rotation.Right, tl.BbIndex - Vector2.UnitX) },
			Shape.O => new List<TetrominoLocked>
			{
				new(Shape.O, Rotation.Up, tl.BbIndex), new(Shape.O, Rotation.Down, tl.BbIndex),
				new(Shape.O, Rotation.Left, tl.BbIndex), new(Shape.O, Rotation.Right, tl.BbIndex)
			},
			Shape.None => throw new ArgumentOutOfRangeException( ),
			_ => new List<TetrominoLocked> { tl }
		};

		return _data.Shapes.Any(checkForMatches.Contains);
	}
}