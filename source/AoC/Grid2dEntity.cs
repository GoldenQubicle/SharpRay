using System.Collections.Concurrent;
using Common.Extensions;

namespace AoC;

internal class Grid2dEntity : Entity
{
	private readonly Texture2D texture;
	private Vector2 CellSizeHalf;
	private List<Button> buttons;
	private int animationSpeed = 5;

	public Grid2dEntity(Grid2d grid, SharpRayConfig config)
	{
		CellSizeHalf = CellSize / 2;

		var image = GenImageChecked(config.WindowWidth, config.WindowHeight, (int)CellSize.X, (int)CellSize.Y, Color.VIOLET, Color.DARKPURPLE);
		texture = LoadTextureFromImage(image);
		UnloadImage(image);
		
		buttons = grid.Select(c => new Button
		{
			Size = CellSize,
			Position = GridPosition2Screen(c.X, c.Y) + CellSizeHalf, //buttons are drawn from the center, so add half the cell size
			HasOutlines = true,
			DoCenterText = true,
			BaseColor = Color.BLANK,
			OutlineColor = ColorAlpha(Color.DARKGREEN, .75f),
			TextColor = Color.GOLD,
			FocusColor = Color.RED,
			FontSize = CellSize.X > CellSize.Y ? CellSize.Y : CellSize.X,
			Text = c.Character.ToString( ),
			OnMouseLeftClick = e => new GridEvent(e, (x: (int)(e.Position.X - CellSizeHalf.X) / (int) CellSize.X, y: (int)(e.Position.Y - CellSizeHalf.Y) / (int)CellSize.Y)),
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
			DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, renderUpdateColor[bag.Key])));
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

	private Vector2 GridPosition2Screen(int x, int y) => new(CellSize.X * x, CellSize.Y * y);

}