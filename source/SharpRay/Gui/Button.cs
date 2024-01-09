using SharpRay.Core;

namespace SharpRay.Gui
{
	public sealed class Button : Label
	{
		public Color FocusColor { get; set; } = GRAY;
		public Color BaseColor { get; set; } = DARKGRAY;
		public Texture2D? Texture2D { get; set; }

		public override void Render()
		{
			if (!IsVisible)
				return;

			FillColor = HasMouseFocus ? FocusColor : BaseColor;

			if (Texture2D is null)
				base.Render( );
			else
				DrawTextureV(Texture2D.Value, Position - Size / 2, FillColor);
		}

		public override bool ContainsPoint(Vector2 point) =>
				point.X > Position.X - Size.X / 2 &&
				point.X < Position.X + Size.X / 2 &&
				point.Y > Position.Y - Size.Y / 2 &&
				point.Y < Position.Y + Size.Y / 2;

		public override void OnMouseEvent(IMouseEvent e)
		{
			if (!IsVisible)
				return;

			base.OnMouseEvent(e);

			if (!HasMouseFocus || e is not MouseLeftClick mlc)
				return;

			// There are scenarios where we do not want to emit an event, yet still want to invoke the delegate. 
			// When EmitEvent is null, the delegate won't be invoked even if assigned hence this null check. 
			if (EmitEvent is not null)
				EmitEvent.Invoke(OnMouseLeftClick?.Invoke(this));
			else
				OnMouseLeftClick?.Invoke(this);

			mlc.IsHandled = true;
		}
	}
}

