namespace AoC.Entities;

internal class PathFindingEntity : AoCEntity
{
    public static readonly Vector2 CellSize = new(10, 10);

    private readonly Texture2D _texture;
    private readonly List<Button> _buttons;

    public PathFindingEntity(Grid2d grid, SharpRayConfig config, string part) : base(config, part)
    {
        var image = GenImageChecked(config.WindowWidth, config.WindowHeight, (int)CellSize.X, (int)CellSize.Y, Color.Violet, Color.DarkPurple);
        _texture = LoadTextureFromImage(image);
        UnloadImage(image);

        //using buttons because of nice highlighting on mouse over..
        _buttons = grid.Select(c => new Button
        {
            Size = CellSize,
            Position = GridPosition2Screen(c.X, c.Y) + CellSize / 2, //buttons are drawn from the center, so add half the cell size
            HasOutlines = true,
            DoCenterText = true,
            BaseColor = Color.Blank,
            OutlineColor = ColorAlpha(Color.DarkGreen, .75f),
            TextColor = Color.Gold,
            FocusColor = Color.Red,
            FontSize = CellSize.X > CellSize.Y ? CellSize.Y : CellSize.X,
            Text = c.Character.ToString(),
        }).ToList();
    }


    public override async Task RenderAction(IRenderState state, int layer, Color color)
    {
        var update = state.Cast<PathFindingRender>().Set.Cast<Grid2d.Cell>().ToList();

        if (!RenderUpdate.TryAdd(layer, update.ToConcurrentBag()))
            RenderUpdate[layer] = update.ToConcurrentBag();

        RenderUpdateColor.TryAdd(layer, color);

        await Task.Delay(AnimationSpeed);
    }

    public override void Render()
    {
        DrawTexture(_texture, 0, 0, Color.White);

        _buttons.ForEach(b => b.Render());

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
            RenderUpdate = new();
    }

    private static Vector2 GridPosition2Screen(int x, int y) => new(CellSize.X * x, CellSize.Y * y);
}