namespace AoC;

internal class Grid2dEntity : Entity
{
	private readonly Texture2D texture;
	private int cellSize;
	private int cellSizeHalf;
	private List<Button> buttons;

	public Grid2dEntity(Grid2d grid)
	{
		cellSize = width / grid.Width;
		cellSizeHalf = cellSize / 2;
		var image = GenImageChecked(width, height, cellSize, cellSize, Color.VIOLET, Color.DARKPURPLE);
		texture = LoadTextureFromImage(image);
		UnloadImage(image);

		buttons = grid.Select(c => new Button
		{
			Size = new Vector2(cellSize, cellSize),
			Position = new Vector2(cellSize * c.X + cellSizeHalf, cellSize * c.Y + cellSizeHalf),
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

	public override void Render()
	{
		DrawTexture(texture, 0, 0, Color.WHITE);

		buttons.ForEach(b => b.Render( ));
	}

	public override void OnMouseEvent(IMouseEvent e)
	{
		buttons.ForEach(b => b.OnMouseEvent(e));
	}
}