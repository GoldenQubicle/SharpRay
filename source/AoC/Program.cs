using System.Reflection;

namespace AoC;

public class Program
{
	public static readonly Vector2 CellSize = new(25, 25);

	private static Solution day;
	private static Grid2d grid;

	private static void Main()
	{
		day = Solution.Initialize("2022", "12");

		day.RenderAction = state =>
			GetEntity<Grid2dEntity>( ).RenderAction(((PathFinding.VisitedSet)state).set.Cast<Grid2d.Cell>(), 0, ColorAlpha(Color.SKYBLUE, .5f));

		grid = (Grid2d)day.GetType( ).GetField("grid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(day);

		var config = new SharpRayConfig
		{
			Name = "Advent of Code 2022 Day 12 part 1",
			WindowWidth = (int)CellSize.X * grid.Width,
			WindowHeight = (int)CellSize.Y * grid.Height,
		};

		Initialize(config);

		//grid = new Grid2d(new PfSolution("grid20x10").Input, diagonalAllowed: false);

		AddEntity(new Grid2dEntity(grid, config));

		Run( );
	}

	
	public static async void GuiEvent(IGuiEvent e)
	{
		if (e is GridEvent ge)
		{
			Console.WriteLine($"you clicked cell with index x: {ge.Position.x} y: {ge.Position.y} ");
			
			await day.SolvePart1();
			
			
			//PathFinding.FloodFill(ge.Position, grid, cell => cell.Character != '#', 
			//	set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>(), 2, ColorAlpha(Color.BEIGE, .25f) ));

			//var start = grid[ge.Position];
			//var target = grid.First(c => c.Character == 'T');
			//PathFinding.BreadthFirstSearch(start, target, grid, 
			//	(c, _) => c.Character != '#',
			//	(c, t) => c.Character == target.Character,
			//	set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>( ), 0, ColorAlpha(Color.LIME, .5f)));

			//PathFinding.UniformCostSearch(start, target, grid,
			//	(c, _) => c.Character != '#',
			//	(c, t) => c.Character == target.Character,
			//	(t, n) => Maths.GetManhattanDistance((t as Grid2d.Cell).Position, (n as Grid2d.Cell).Position),
			//	set => GetEntity<Grid2dEntity>( ).RenderAction(set.Cast<Grid2d.Cell>( ), 1, ColorAlpha(Color.YELLOW, .5f)));
		}
	}

	public record GridEvent(GuiEntity GuiEntity, (int x, int y) Position) : IGuiEvent;

}