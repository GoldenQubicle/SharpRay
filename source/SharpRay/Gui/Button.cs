﻿using Raylib_cs;
using SharpRay.Core;
using SharpRay.Eventing;
using System;
using System.Numerics;

namespace SharpRay.Gui
{
    public sealed class Button : Label
    {
        public Color FocusColor { get; set; } = Color.GRAY;
        public Color BaseColor { get; set; }
        
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
            if (HasMouseFocus && e is MouseLeftClick mlc)
            {
                EmitEvent?.Invoke(OnMouseLeftClick?.Invoke(this));
                mlc.IsHandled = true;
            }
        }
    }
}

