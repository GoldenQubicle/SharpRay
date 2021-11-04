using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;

namespace SharpRay.Gui
{
    public abstract class DragEditShape : GuiEntity
    {
        public Action<DragEditShape> OnRightMouseClick { get; set; }
        public Color ColorDefault { get; set; }
        public Color ColorFocused { get; set; }
        protected Color ColorRender { get; set; }

        protected bool IsDragged { get; set; }
        private Vector2 DragStart { get; set; }
        public Vector2 DragOffSet { get; private set; }

        public override void OnMouseEvent(IMouseEvent me)
        {
            HasMouseFocus = ContainsPoint(me.Position);

            if (IsDragged)
                Position = me.Position + DragOffSet;

            if (!HasMouseFocus) return;

            if (me is MouseLeftClick && OnMouseLeftClick is not null)
                EmitEvent(OnMouseLeftClick(this));

            if (me is MouseRightClick)
            {
                OnRightMouseClick?.Invoke(this);
            }

            if (me is MouseLeftDrag && !IsDragged)
            {
                DragStart = Position;
                DragOffSet = Position - me.Position;
                IsDragged = true;
            }

            if (me is MouseLeftRelease && IsDragged)
            {
                EmitEvent(new TranslateEdit
                {
                    GuiEntity = this,
                    Start = DragStart,
                    End = me.Position
                });
                IsDragged = false;
            }

            if (me is MouseWheelUp || me is MouseWheelDown)
            {
                var start = Scale;
                Scale += me is MouseWheelUp ? 0.15f : -0.15f;
                EmitEvent(new ScaleEdit { GuiEntity = this, Start = start, End = Scale });
            }
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (!HasMouseFocus) return;

            if (ke is KeyDelete)
                EmitEvent(new DeleteEdit { GuiEntity = this });
        }

        public override void Render()
        {
            ColorRender = HasMouseFocus ? ColorFocused : ColorDefault;
        }
    }
}

