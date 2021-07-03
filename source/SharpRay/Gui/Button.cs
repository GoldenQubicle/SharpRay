using Raylib_cs;
using SharpRay.Core;
using SharpRay.Eventing;
using System.Numerics;

namespace SharpRay.Gui
{
    public sealed class Button : Label
    {
        public Color FocusColor { get; init; }
        public Color BaseColor { get; init; }
        public override void Render()
        {
            FillColor = HasMouseFocus ? FocusColor : BaseColor;
            base.Render();
        }

        public override bool ContainsPoint(Vector2 point) =>
                point.X > Position.X &&
                point.X < Position.X + Size.X &&
                point.Y > Position.Y &&
                point.Y < Position.Y + Size.Y;

        public override void OnMouseEvent(IMouseEvent e)
        {
            base.OnMouseEvent(e);
            if (HasMouseFocus && e is MouseLeftClick)
                EmitEvent(OnMouseLeftClick(this));
        }
    }
}

