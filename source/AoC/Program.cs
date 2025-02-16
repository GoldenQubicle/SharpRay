using AoC.Entities;

namespace AoC;

/* days visualized
 * PATH FINDING
 * 2022 12 1
 * 2021 15 1 note part 2 doesn't work since we expand the grid in the solution...
 * KNOT HASH
 * 2017 10 2
 */

public class Program
{
	private static Solution solution;

	private static void Main()
	{
		Initialize2018Day15("2018", "15", "1");

		//InitializePathFindingEntity("2021", "15", "1");

		//InitializeKnotHashEntity("2017", "10", "2");

		Run( );
	}

	private static void Initialize2018Day15(string year, string day, string part)
	{
		solution = Solution.Initialize(year, day);

		IRenderState.Update = state => GetEntity<CombatEntity>( ).RenderAction(state);
		var config = new SharpRayConfig
		{
			Name = $"Advent of Code {year} Day {day} part {part}",
			WindowHeight = 1024,
			WindowWidth = 1024,
			BackGroundColor = Color.DarkBlue
		};

		Initialize(config);
		var grid = (Grid2d)solution.GetType( ).GetField("grid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solution);
		var units = (List<UnitData>)solution.GetType( ).GetField("unitData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solution);
		AddEntity(new CombatEntity(grid, units, config, part));
	}

	private static void InitializeKnotHashEntity(string year, string day, string part)
	{
		solution = Solution.Initialize(year, day);
		
		IRenderState.Update = state => GetEntity<KnotHashEntity>( ).RenderAction(state);

		var config = new SharpRayConfig
		{
			Name = $"Advent of Code {year} Day {day} part {part}",
			WindowHeight = 1024,
			WindowWidth = 860,
			BackGroundColor = Color.DarkBlue
		};

		Initialize(config);
		AddEntity(new KnotHashEntity(config, part));
	}


	private static void InitializePathFindingEntity(string year, string day, string part)
	{
		solution = Solution.Initialize(year, day);

		IRenderState.Update = state =>
			GetEntity<PathFindingEntity>( ).RenderAction(state, 0, ColorAlpha(Color.SkyBlue, .5f));

		//assuming a path finding solution has a grid field...
		var grid = (Grid2d)solution.GetType( ).GetField("grid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solution);

		var config = new SharpRayConfig
		{
			Name = $"Advent of Code {year} Day {day} part {part}",
			WindowWidth = (int)PathFindingEntity.CellSize.X * grid.Width,
			WindowHeight = (int)PathFindingEntity.CellSize.Y * grid.Height,
		};

		Initialize(config);
		AddEntity(new PathFindingEntity(grid, config, part));
	}


	public static async void GuiEvent(IGuiEvent e)
	{
		if (e is AoCEvent aoc)
		{
			aoc.GuiEntity.Hide( );
			if (aoc.Part.Equals("1"))
				await solution.SolvePart1( );
			else
				await solution.SolvePart2( );
		}
	}

	public record AoCEvent(GuiEntity GuiEntity, string Part) : IGuiEvent;

}