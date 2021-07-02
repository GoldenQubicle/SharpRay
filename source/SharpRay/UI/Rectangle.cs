using static Raylib_cs.Raylib;
using System.Numerics;

namespace SharpRay
{
    public sealed class Rectangle : DragEditShape
    {
        public override bool ContainsPoint(Vector2 point) =>
                point.X > Position.X &&
                point.X < Position.X + Size.X * Scale &&
                point.Y > Position.Y &&
                point.Y < Position.Y + Size.Y * Scale;

        public override void Render()
        {
            base.Render();
            DrawRectangleV(Position, Size * new Vector2(Scale, Scale), ColorRender);
        }
    }
}

