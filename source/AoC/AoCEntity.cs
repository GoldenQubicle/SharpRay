namespace AoC
{
	internal abstract class AoCEntity : Entity
	{
		protected readonly int AnimationSpeed = 5;

		protected ConcurrentDictionary<int, ConcurrentBag<Grid2d.Cell>> RenderUpdate = new( );

		protected readonly ConcurrentDictionary<int, Color> RenderUpdateColor = new( );

		public abstract Task RenderAction(IRenderState state, int layer = 0, Color color = default );

		protected AoCEntity(SharpRayConfig config)
		{
			AddEntity(new Button
			{
				RenderLayer = 2,
				Text = "Start",
				DoCenterText = true,
				FontSize = 20,
				Size = new Vector2(100, 30),
				Position = new Vector2(config.WindowWidth - 75, config.WindowHeight - 40 ),
				TextColor = Color.RAYWHITE,
				FocusColor = Color.GREEN,
				BaseColor = Color.DARKGREEN,
				OnMouseLeftClick = e => new AoCEvent(e),
				EmitEvent = GuiEvent
			});
		}
	}
}
