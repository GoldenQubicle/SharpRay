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
            FillColor = HasMouseFocus ? FocusColor : BaseColor;
            
            if (Texture2D is null)
                base.Render();
            else
                DrawTextureV(Texture2D.Value, Position - Size/2, FillColor);
        }

        public override bool ContainsPoint(Vector2 point) =>
                point.X > Position.X - Size.X / 2 &&
                point.X < Position.X + Size.X / 2 &&
                point.Y > Position.Y - Size.Y / 2 &&
                point.Y < Position.Y + Size.Y / 2;

        public override void OnMouseEvent(IMouseEvent e)
        {
            base.OnMouseEvent(e);
            if (HasMouseFocus && e is MouseLeftClick mlc)
            {
                EmitEvent?.Invoke(OnMouseLeftClick?.Invoke(this));
                mlc.IsHandled = true;
            }
        }
    }
}

