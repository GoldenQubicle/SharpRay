namespace TerribleTetris;

internal class Pattern : Entity, IHasRender
{
	private PatternData _data;
	private readonly Vector2 _cellSize;
	private readonly List<Vector2> _cells;

	public Pattern(PatternData data, GridData gridData)
	{
		_data = data;
		_cellSize = new(gridData.CellSize, gridData.CellSize);
		_cells = _data.Shapes
			.SelectMany(s => Tetromino.GetOffsets(s.Shape, s.Rotation)
				.Select(o => OffsetToScreen(s.BbIndex, o))).ToList( );

	}

	public override void Render()
	{
		foreach (var pos in _cells)
		{
			DrawRectangleV(pos, _cellSize, BEIGE);
		}
	}

	public void CalculateScore(TetrominoLocked tl)
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
			Shape.O => new List<TetrominoLocked>{ new (Shape.O, Rotation.Up, tl.BbIndex), new(Shape.O, Rotation.Down, tl.BbIndex),
												  new (Shape.O, Rotation.Left, tl.BbIndex), new(Shape.O, Rotation.Right, tl.BbIndex) },
			Shape.None => throw new ArgumentOutOfRangeException( ),
			_ => new List<TetrominoLocked> { tl }
		};


		if (_data.Shapes.Any(checkForMatches.Contains))
		{
			Print("we have a match!");
		}
	}
}