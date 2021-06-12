using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace SharpRay
{
    public abstract class UIEntity : Entity, IEventEmitter<IUIEvent>
    {
        //not too sure about these 2...          
        public Func<UIEntity, IUIEvent> OnMouseLeftClick { get; set; }
        public Action<UIEntity> OnRightMouseClick { get; set; }

        public Action<IUIEvent> EmitEvent { get; set; }

        public float Scale { get; set; } = 1f;
        public Color BaseColor { get; set; }
        public Color HighLightColor { get; set; } = Color.BROWN;
        public bool HasMouseFocus { get; protected set; }
        public abstract bool ContainsPoint(Vector2 point);

        protected Color Color { get; private set; }
        protected bool IsDragged { get; set; }
        private Vector2 DragStart { get; set; }
        private Vector2 DragOffSet { get; set; }

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
                EmitEvent(new ScaleEdit { UIComponent = this, Start = start, End = Scale });
            }
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (!HasMouseFocus) return;

            if (ke is KeyDelete)
                EmitEvent(new DeleteEdit { UIComponent = this });
        }

        public override void Render(double deltaTime) => Color = HasMouseFocus ? HighLightColor : BaseColor;

    }

    /*
     * kinda experimental class 
     *  2 types of event emitter, ui & audio
     *  OnDrawText func to update the text
     *  uses OnMouseLeftClick to get the event to emit
     */
    public class ToggleButton : Rectangle, IEventEmitter<IAudioEvent>
    {
        Action<IAudioEvent> IEventEmitter<IAudioEvent>.EmitEvent { get; set; }

        public Func<string> OnDrawText { get; set; }

        public bool IsToggled { get; private set; }

        public override void Render(double deltaTime)
        {
            base.Render(deltaTime);
            DrawText(OnDrawText(), (int)Position.X, (int)Position.Y + 2, 25, Color.WHITE);
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            HasMouseFocus = ContainsPoint(me.Position);

            if (!HasMouseFocus) return;

            if (me is MouseLeftClick)
            {
                IsToggled = !IsToggled;
                EmitEvent(OnMouseLeftClick(this)); // nre risk on purpose, need to have an event to emit for a functional button
                (this as IEventEmitter<IAudioEvent>).EmitEvent(new AudioToggleTimerClicked { Entity = this }); //erhm, not great
            }
        }
    }

    public class Circle : UIEntity
    {
        public float Radius { get; set; }

        public override bool ContainsPoint(Vector2 point) => Vector2.Distance(Position, point) < Radius * Scale;

        public override void Render(double deltaTime)
        {
            base.Render(deltaTime);
            DrawCircleV(Position, Radius * Scale, Color);
        }
    }

    public class Rectangle : UIEntity
    {
        public override bool ContainsPoint(Vector2 point) =>
                point.X > Position.X &&
                point.X < Position.X + Size.X * Scale &&
                point.Y > Position.Y &&
                point.Y < Position.Y + Size.Y * Scale;

        public override void Render(double deltaTime)
        {
            base.Render(deltaTime);
            DrawRectangleV(Position, Size * new Vector2(Scale, Scale), Color);
        }
    }


    public class Polygon : UIEntity
    {
        //points are ordered anti-clockwise and are drawn relative to Position, i.e. as if Position were 0,0
        //in addition Position acts as the 1st point and opens/closes the polygon
        public Vector2[] Points { get; init; }
        public Vector2[] TextCoords { get; init; }

        private static Texture2D texture2D = new() { id = 1, }; // seems like textures need to be unloaded btw

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }

        public override void Render(double deltaTime)
        {
            base.Render(deltaTime);
            DrawTexturePoly(texture2D, Position, Points, TextCoords, Points.Length, Color);
        }
    }
}

