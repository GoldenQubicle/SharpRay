using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace SharpRay
{
    public abstract class UIComponent : Entity, IUIEventEmitter, ILoop
    {
        public Action<IUIEvent> EmitUIEvent { get; set; }

        //not too sure about these 2...          
        public Func<UIComponent, IUIEvent> OnMouseLeftClick { get; set; } 
        public Action<UIComponent> OnRightMouseClick { get; set; }


        public Vector2 Position { get; set; }
        public float Scale { get; set; } = 1f;
        public Color BaseColor { get; set; }
        public Color HighLightColor { get; set; } = Color.BROWN;
        public bool HasMouseFocus { get; protected set; }
        public abstract bool ContainsPoint(Vector2 point);

        protected Color Color { get; private set; }
        protected bool IsDragged { get; private set; }
        private Vector2 DragStart { get; set; }
        private Vector2 DragOffSet { get; set; }

        public virtual void Update(Vector2 mPos)
        {
            HasMouseFocus = ContainsPoint(mPos);
            Color = HasMouseFocus ? HighLightColor : BaseColor;

            if (IsDragged)
                Position = mPos + DragOffSet;
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            if (!HasMouseFocus) return;

            if (me is MouseLeftClick && OnMouseLeftClick is not null)
                EmitUIEvent(OnMouseLeftClick(this));

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
                EmitUIEvent(new TranslateEdit
                {
                    UIComponent = this,
                    Start = DragStart,
                    End = me.Position
                });
                IsDragged = false;
            }

            if (me is MouseWheelUp || me is MouseWheelDown)
            {
                var start = Scale;
                Scale += me is MouseWheelUp ? 0.15f : -0.15f;
                EmitUIEvent(new ScaleEdit { UIComponent = this, Start = start, End = Scale });
            }
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (!HasMouseFocus) return;

            if (ke is KeyDelete)
                EmitUIEvent(new DeleteEdit { UIComponent = this });
        }

    }

    /*
     * kinda experimental class 
     *  2 types of event emitter, ui & audio
     *  OnUpdate func to update the timer text
     *  uses OnMouseLeftClick to get the event to emit
     */
    public class ToggleButton : Rectangle, IAudioEventEmitter
    {
        public string Text { get; set; }
        public Func<string> OnUpdate { get; set; }

        public bool IsToggled { get; private set; }

        public override void Update(Vector2 mPos)
        {
            base.Update(mPos);
            Text = OnUpdate();
        }
        public override void Draw()
        {
            base.Draw();
            DrawText(Text, (int)Position.X, (int)Position.Y + 2, 25, Color.WHITE);
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            //not calling base since we don't need dragging nor scaling for a static button

            if (!HasMouseFocus) return;

            if (me is MouseLeftClick)
            {
                IsToggled = !IsToggled;
                EmitUIEvent(OnMouseLeftClick(this)); // nre risk on purpose, need to have an event to emit for a functional button
                EmitAdudioEvent(new AudioToggleTimerClicked { Entity = this }); //erhm, not great
            }
        }

        public Action<IAudioEvent> EmitAdudioEvent { get; set; }
    }

    public class Circle : UIComponent
    {
        public float Radius { get; set; }

        public override bool ContainsPoint(Vector2 point)
        {
            var d = Vector2.Distance(Position, point);
            return d < Radius * Scale;
        }

        public override void Draw() => DrawCircleV(Position, Radius * Scale, Color);
    }

    public class Rectangle : UIComponent
    {
        public Vector2 Size { get; set; }

        public override bool ContainsPoint(Vector2 point)
        {
            return point.X > Position.X && point.X < Position.X + Size.X * Scale &&
                 point.Y > Position.Y && point.Y < Position.Y + Size.Y * Scale;
        }

        public override void Draw() => DrawRectangleV(Position, Size * new Vector2(Scale, Scale), Color);
    }


    public class Polygon : UIComponent
    {
        //points are ordered anti-clockwise and are drawn relative to Position, i.e. as if it were 0,0
        //in addition Position acts as the 1st point and opens/closes the polygon
        public Vector2[] Points { get; init; }
        public Vector2[] TextCoords { get; init; }

        private static Texture2D texture2D = new() { id = 1, }; // seems like textures need to be unloaded btw

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }

        public override void Draw() => DrawTexturePoly(texture2D, Position, Points, TextCoords, Points.Length, Color);
    }
}

