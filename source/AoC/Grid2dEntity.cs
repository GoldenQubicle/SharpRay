using System.Collections.Concurrent;
using Common.Extensions;

namespace AoC;

internal class Grid2dEntity : Entity
{
	private readonly Texture2D texture;
	private int cellSize;
	private int cellSizeHalf;
	private List<Button> buttons;
	private int animationSpeed = 5;

	public Grid2dEntity(Grid2d grid)
	{
		cellSize = Width / grid.Width;
		cellSizeHalf = cellSize / 2;
		var image = GenImageChecked(Width, Height, cellSize, cellSize, Color.VIOLET, Color.DARKPURPLE);
		texture = LoadTextureFromImage(image);
		UnloadImage(image);

		buttons = grid.Select(c => new Button
		{
			Size = new Vector2(cellSize, cellSize),
			Position = GridPosition2Screen(c.X, c.Y) + new Vector2(cellSizeHalf, cellSizeHalf), //buttons are drawn from the center, so add half the cell size
			HasOutlines = true,
			DoCenterText = true,
			BaseColor = Color.BLANK,
			TextColor = Color.ORANGE,
			FocusColor = Color.RED,
			FontSize = cellSize,
			Text = c.Character.ToString( ),
			OnMouseLeftClick = e => new GridEvent(e, (x: (int)(e.Position.X - cellSizeHalf) / cellSize, y: (int)(e.Position.Y - cellSizeHalf) / cellSize)),
			EmitEvent = GuiEvent,
		}).ToList( );
	}

	private ConcurrentDictionary<int, ConcurrentBag<Grid2d.Cell>> renderUpdate = new();
	private ConcurrentDictionary<int, Color> renderUpdateColor = new();

	public async Task RenderAction(IEnumerable<Grid2d.Cell> update, int layer, Color color)
	{
		if (!renderUpdate.TryAdd(layer, update.ToConcurrentBag()))
			renderUpdate[layer] = update.ToConcurrentBag();

		renderUpdateColor.TryAdd(layer, color);

		await Task.Delay(animationSpeed);
	}

	public override void Render()
	{
		DrawTexture(texture, 0, 0, Color.WHITE);

		buttons.ForEach(b => b.Render( ));

		renderUpdate.ForEach(bag => bag.Value.ForEach(c => 
			DrawRectangleV(GridPosition2Screen(c.X, c.Y), new Vector2(cellSize, cellSize), renderUpdateColor[bag.Key])));
	}

	public override void OnMouseEvent(IMouseEvent e)
	{
		buttons.ForEach(b => b.OnMouseEvent(e));
	}

	public override void OnKeyBoardEvent(IKeyBoardEvent e)
	{
		if (e is KeySpaceBarDown)
			renderUpdate = new();
	}

	private Vector2 GridPosition2Screen(int x, int y) => new(cellSize * x, cellSize * y);

}