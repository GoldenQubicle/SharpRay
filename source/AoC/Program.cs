namespace AoC;

public class Program
{
	private static Grid2d grid;
	public const int Height = 720;
	public const int Width = 720;

	private static void Main()
	{
		var config = new SharpRayConfig
		{
			WindowWidth = Width,
			WindowHeight = Height
		};

		Initialize(config);

		grid = new Grid2d(new PfSolution("grid20x20").Input, diagonalAllowed: false);

		AddEntity(new Grid2dEntity(grid));

		Run( );
	}

	
	public static void GuiEvent(IGuiEvent e)
	{
		if (e is GridEvent ge)
		{
			Console.WriteLine($"you clicked cell with index x: {ge.Position.x} y: {ge.Position.y} ");

			PathFinding.FloodFill(ge.Position, grid, cell => cell.Character != '#', 
				set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>(), 2, ColorAlpha(Color.BEIGE, .25f) ));

			var start = grid[ge.Position];
			var target = grid.First(c => c.Character == 'T');
			PathFinding.BreadthFirstSearch(start, target, grid, 
				(c, _) => c.Character != '#',
				(c, t) => c.Character == target.Character,
				set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>( ), 0, ColorAlpha(Color.LIME, .5f)));

			PathFinding.UniformCostSearch(start, target, grid,
				(c, _) => c.Character != '#',
				(c, t) => c.Character == target.Character,
				(t, n) => Maths.GetManhattanDistance((t as Grid2d.Cell).Position, (n as Grid2d.Cell).Position),
				set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>( ), 1, ColorAlpha(Color.YELLOW, .5f)));
		}
	}

	public record GridEvent(GuiEntity GuiEntity, (int x, int y) Position) : IGuiEvent;

}