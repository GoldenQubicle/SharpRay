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

			FloodFill.Run(ge.Position, grid, cell => cell.Character != '#', set => GetEntity<Grid2dEntity>( ).RenderAction(set));
		}
	}

	public record GridEvent(GuiEntity GuiEntity, (int x, int y) Position) : IGuiEvent;

}