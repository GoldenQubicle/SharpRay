namespace TerribleTetris;

internal class Pattern : Entity, IHasRender
{
	private readonly Vector2 _cellSize;
	private readonly List<Vector2> _cells;

	public Pattern(PatternData data, GridData gridData)
	{
		_cellSize = new(gridData.CellSize, gridData.CellSize);
		_cells = data.Shapes
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
}