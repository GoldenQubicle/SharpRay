namespace TerribleTetris;

internal class Pattern : Entity, IHasRender
{
	private readonly Vector2 _cellSize = new(Game.GridData.CellSize, Game.GridData.CellSize);
	private readonly List<Vector2> _cells;

	public Pattern(PatternData data)
	{
		_cells = data.Shapes
			.SelectMany(s => TetrominoData.GetOffsets(s.Shape, s.Rotation)
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