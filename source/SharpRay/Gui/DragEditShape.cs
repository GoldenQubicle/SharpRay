using SharpRay.Core;

namespace SharpRay.Gui
{
    public abstract class DragEditShape : GuiEntity
    {
        public Action<DragEditShape> OnRightMouseClick { get; set; }
        public Color ColorDefault { get; set; }
        public Color ColorFocused { get; set; }
        public bool CanScale { get; set; }
        protected Color ColorRender { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsDragged { get; private set; }
        private Vector2 DragOffSet { get; set; }
        private Vector2 DragStart { get; set; }
        
        /// <summary>
        /// NOTE: the event returned by the func MUST also implement IHasUndoRedo in order to work properly
        /// </summary>
        public Func<GuiEntity, IGuiEvent> OnDelete { get; set; }

        public override void OnMouseEvent(IMouseEvent me)
        {
            HasMouseFocus = ContainsPoint(me.Position);

            if (IsDragged && IsSelected)
            {
                Position = me.Position + DragOffSet;
            }

            if (!HasMouseFocus)
            {
                return;
            }

            if (me is MouseLeftClick )
            {
                IsSelected = true;
                if(OnMouseLeftClick is not null)
                    EmitEvent(OnMouseLeftClick(this));
            }

            if (me is MouseRightClick)
            {
                OnRightMouseClick?.Invoke(this);
            }

            if (me is MouseLeftDrag && !IsDragged && IsSelected)
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
                    End = Position
                });
                IsDragged = false;
                IsSelected = false;
            }

            if (CanScale && ( me is MouseWheelUp || me is MouseWheelDown))
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
                EmitEvent(OnDelete?.Invoke(this));
        }

        public override void Render()
        {
            ColorRender = HasMouseFocus ? ColorFocused : ColorDefault;
        }
    }
}

