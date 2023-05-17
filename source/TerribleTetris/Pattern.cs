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
				.Select(o => OffsetToScreen(s.BbIndex, o))).ToList();

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
		//due to rotation of I, S & Z we need to check both (up & down) and (left & right) because placement is the same.
		var checkForMatches = tl.Shape switch
		{
			Shape.I when tl.Rotation == Rotation.Up => new List<TetrominoLocked>{ tl, new(Shape.I, Rotation.Down, tl.BbIndex - Vector2.UnitY) },
			Shape.I when tl.Rotation == Rotation.Down => new List<TetrominoLocked>{ tl, new(Shape.I, Rotation.Up, tl.BbIndex + Vector2.UnitY) },
			Shape.I when tl.Rotation == Rotation.Right => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Left, tl.BbIndex - Vector2.UnitX) },
			Shape.I when tl.Rotation == Rotation.Left => new List<TetrominoLocked> { tl, new(Shape.I, Rotation.Right, tl.BbIndex + Vector2.UnitX) },
			//Shape.O => expr,
			//Shape.T => expr,
			//Shape.J => expr,
			//Shape.L => expr,
			//Shape.S => expr,
			//Shape.Z => expr,
			_ => throw new ArgumentOutOfRangeException()
		};

		//var toCheck = new List<TetrominoLocked> { tl };
		
		//if (tl.Shape == Shape.I)
		//{
		//	toCheck.Add(tl.Rotation == Rotation.Up
		//		? new TetrominoLocked(Shape.I, Rotation.Down, tl.BbIndex - Vector2.UnitY)
		//		: new TetrominoLocked(Shape.I, Rotation.Up, tl.BbIndex + Vector2.UnitY));
		//}

		if(_data.Shapes.Any(checkForMatches.Contains))
		{
			Print("we have a match!");

		}

	}
}