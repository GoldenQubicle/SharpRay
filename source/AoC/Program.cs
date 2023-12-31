namespace AoC;

public class Program
{
	public const int height = 720;
	public const int width = 720;
	const int cells = 72;

	static void Main(string[ ] args)
	{
		var config = new SharpRayConfig
		{
			WindowWidth = width,
			WindowHeight = height
		};

		Initialize(config);

		var grid = new Grid2d(new PfSolution("grid20x20").Input);

		AddEntity(new Grid2dEntity(grid));

		Run( );
	}

	public static void GuiEvent(IGuiEvent e)
	{
		if (e is GridEvent ge)
		{
			Console.WriteLine($"you clicked cell with index x: {ge.Position.x} y: {ge.Position.y} ");
		}
	}

	public record GridEvent(GuiEntity GuiEntity, (int x, int y) Position) : IGuiEvent;

}