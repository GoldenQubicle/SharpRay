namespace AoC;

/* days visualized
 * 2022 12 1
 * 2017 10 2
 */

public class Program
{
	private const string Year = "2017";
	private const string Day = "10";
	private const string Part = "2";
	private static Solution solution;

	private static void Main()
	{
		//var setup = SetupGridEntity( );
		//solution = setup.solution;
		//Initialize(setup.config);
		//AddEntity(new Grid2dEntity(setup.grid, setup.config));

		var config = new SharpRayConfig
		{
			Name = $"Advent of Code {Year} Day {Day} part {Part}",
			WindowHeight = 1024,
			WindowWidth = 860,
			BackGroundColor = Color.DARKBLUE
		};

		Initialize(config);

		solution = Solution.Initialize(Year, Day);
		solution.RenderAction = state => GetEntity<KnotHashEntity>( ).RenderAction(state);

		AddEntity(new KnotHashEntity(config));

		Run( );
	}


	private static (SharpRayConfig config, Solution solution, Grid2d grid) SetupGridEntity()
	{
		var solution = Solution.Initialize(Year, Day);

		solution.RenderAction = state =>
			GetEntity<Grid2dEntity>( ).RenderAction(state, 0, ColorAlpha(Color.SKYBLUE, .5f));

		var grid = (Grid2d)solution.GetType( ).GetField("grid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solution);

		var config = new SharpRayConfig
		{
			Name = $"Advent of Code {Year} Day {Day} part {Part}",
			WindowWidth = (int)Grid2dEntity.CellSize.X * grid.Width,
			WindowHeight = (int)Grid2dEntity.CellSize.Y * grid.Height,
		};
		
		return (config, solution, grid);
	}

	
	public static async void GuiEvent(IGuiEvent e)
	{
		if (e is AoCEvent aoc)
		{
			aoc.GuiEntity.Hide();
			if (Part.Equals("1"))
				await solution.SolvePart1();
			else
				await solution.SolvePart2();
		}
	}

	public record AoCEvent(GuiEntity GuiEntity) : IGuiEvent;

}