namespace TerribleTetris;

internal class Pattern : Entity, IHasRender
{
	private readonly Vector2 _cellSize = new(Game.GridData.CellSize, Game.GridData.CellSize);
	private readonly List<Vector2> _positions;

	public Pattern(PatternData data)
	{
		_positions = data.Shapes
			.SelectMany(s => TetrominoData.GetOffsets(s.Shape, s.Rotation)
				.Select(o => OffsetToScreen(s.BbIndex, o))).ToList();

	}

	public override void Render()
	{
		foreach (var pos in _positions)
		{
			DrawRectangleV(pos, _cellSize, BEIGE);
		}
	}
}