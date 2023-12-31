using Common.Extensions;

namespace AoC;

internal class Grid2dEntity : Entity
{
	private readonly Texture2D texture;
	private int cellSize;
	private int cellSizeHalf;
	private List<Button> buttons;
	private Action DoRenderAction;
	private Queue<Action> renderActions = new();


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

	public async Task RenderAction(IEnumerable<Grid2d.Cell> set)
	{
		DoRenderAction = () =>
			set.ForEach(c =>
				DrawRectangleV(GridPosition2Screen(c.X, c.Y), new Vector2(cellSize, cellSize), Color.MAROON));
		await Task.Delay(5);
		
	}

	public override void Render()
	{
		DrawTexture(texture, 0, 0, Color.WHITE);

		buttons.ForEach(b => b.Render( ));

		DoRenderAction?.Invoke( );
	}

	public override void OnMouseEvent(IMouseEvent e)
	{
		buttons.ForEach(b => b.OnMouseEvent(e));
	}

	private Vector2 GridPosition2Screen(int x, int y) => new(cellSize * x, cellSize * y);

}