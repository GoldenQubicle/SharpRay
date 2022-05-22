﻿namespace SharpRay.Gui
{
    public sealed class PointHandler : DragEditShape
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

