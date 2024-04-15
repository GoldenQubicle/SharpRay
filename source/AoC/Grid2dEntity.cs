﻿using Common.Renders;

namespace AoC;

internal class Grid2dEntity : AoCEntity
{
	public static readonly Vector2 CellSize = new(15, 15);

	private readonly Texture2D _texture;
	private readonly List<Button> _buttons;
	
	public Grid2dEntity(Grid2d grid, SharpRayConfig config) : base(config)
	{
		var image = GenImageChecked(config.WindowWidth, config.WindowHeight, (int)CellSize.X, (int)CellSize.Y, Color.VIOLET, Color.DARKPURPLE);
		_texture = LoadTextureFromImage(image);
		UnloadImage(image);

		//using buttons because of nice highlighting on mouse over..
		_buttons = grid.Select(c => new Button
		{
			Size = CellSize,
			Position = GridPosition2Screen(c.X, c.Y) + CellSize / 2, //buttons are drawn from the center, so add half the cell size
			HasOutlines = true,
			DoCenterText = true,
			BaseColor = Color.BLANK,
			OutlineColor = ColorAlpha(Color.DARKGREEN, .75f),
			TextColor = Color.GOLD,
			FocusColor = Color.RED,
			FontSize = CellSize.X > CellSize.Y ? CellSize.Y : CellSize.X,
			Text = c.Character.ToString( ),
		}).ToList( );
	}


	public override async Task RenderAction(IRenderState state, int layer, Color color)
	{
		//note we're assuming a cast to the path finding renderer
		//this will cause some future headache when for instance wanting to render cellular automata
		var update = state.Cast<PathFindingRender>().Set.Cast<Grid2d.Cell>( ).ToList();

		if (!RenderUpdate.TryAdd(layer, update.ToConcurrentBag( )))
			RenderUpdate[layer] = update.ToConcurrentBag( );

		RenderUpdateColor.TryAdd(layer, color);

		await Task.Delay(AnimationSpeed);
	}

	public override void Render()
	{
		DrawTexture(_texture, 0, 0, Color.WHITE);

		_buttons.ForEach(b => b.Render( ));

		RenderUpdate.ForEach(bag => bag.Value.ForEach(c =>
			DrawRectangleV(GridPosition2Screen(c.X, c.Y), CellSize, RenderUpdateColor[bag.Key])));
	}

	public override void OnMouseEvent(IMouseEvent e)
	{
		_buttons.ForEach(b => b.OnMouseEvent(e));
	}

	public override void OnKeyBoardEvent(IKeyBoardEvent e)
	{
		if (e is KeySpaceBarDown)
			RenderUpdate = new( );
	}

	private static Vector2 GridPosition2Screen(int x, int y) => new(CellSize.X * x, CellSize.Y * y);
}