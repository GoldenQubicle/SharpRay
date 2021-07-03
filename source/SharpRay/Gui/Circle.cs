using static Raylib_cs.Raylib;
using System.Numerics;

namespace SharpRay.Gui
{
    public sealed class Circle : DragEditShape
    {
        public float Radius { get; set; }

        public override bool ContainsPoint(Vector2 point) => Vector2.Distance(Position, point) < Radius * Scale;

        public override void Render()
        {
            base.Render();
            DrawCircleV(Position, Radius * Scale, ColorRender);
        }
    }
}

