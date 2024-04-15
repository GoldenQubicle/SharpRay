namespace AoC.Entities;

internal class CombatEntity(SharpRayConfig config, string part) : AoCEntity<CombatRender>(config, part)
{
	public static Vector2 CellSize = new(10, 10);

	private ConcurrentDictionary<int, CombatRender> renderstates = new();

	public override async Task RenderAction(CombatRender state, int layer = 0, Color color = default)
	{
		CellSize = new(config.WindowWidth / state.Grid.Width, config.WindowHeight / state.Grid.Height);
		renderstates.TryAdd(0, state);
		

		await Task.Delay(AnimationSpeed);
	}

	public override void Render()
	{
		if (renderstates.IsEmpty) return;

		renderstates[0].Grid.ForEach(c =>
		{
			
			if (c.Character == '#')
				DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, Color.BROWN);
			if (c.Character == '.')
				DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, Color.RAYWHITE);
			if (c.Character == 'G')
				DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, Color.DARKGREEN);
			if (c.Character == 'E')
				DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, Color.SKYBLUE);

			DrawRectangleLinesV(GridPosition2Screen(c.X, c.Y), CellSize, Color.GRAY);
		});
	}

	private static Vector2 GridPosition2Screen(int x, int y) => new(CellSize.X * x, CellSize.Y * y);
}