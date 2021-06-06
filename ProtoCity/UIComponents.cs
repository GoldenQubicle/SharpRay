using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace ProtoCity
{
    public abstract class UIComponent
    {
        public Vector2 Position { get; set; }
        public Color BaseColor { get; set; }
        public Color HighLightColor { get; set; } = Color.BROWN;
        public bool HasMouseFocus { get; protected set; }
        public abstract bool ContainsPoint(Vector2 point);
        public abstract void Draw();
        
        protected Color Color { get; private set; }
        protected bool IsDragged { get;  private set; }
        private Vector2 DragStart { get; set; }
        private Vector2 DragOffSet { get; set; }

        private static readonly Action<IEditEvent> OnEdit = e => Program.UIEventHandler(e);

        public virtual void Update(Vector2 mPos)
        {
            HasMouseFocus = ContainsPoint(mPos);
            Color = HasMouseFocus ? HighLightColor : BaseColor;

            if (IsDragged)
                Position = mPos + DragOffSet;
        }

        public virtual void OnMouseEvent(IMouseEvent me)
        {
            if (!HasMouseFocus) return;

            if (me is MouseLeftDrag && !IsDragged)
            {
                DragStart = Position;
                DragOffSet = Position - me.Position;
                IsDragged = true;
            }

            if (me is MouseLeftRelease && IsDragged)
            {
                OnEdit(new TranslateEdit
                {
                    UIComponent = this,
                    Start = DragStart,
                    End = me.Position
                });
                IsDragged = false;
            }
        }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (!HasMouseFocus) return;

            if (ke is KeyDelete)
                OnEdit(new DeleteEdit { UIComponent = this });
        }
    }

    public class Circle : UIComponent
    {
        public float Radius { get; set; }

        public override bool ContainsPoint(Vector2 point)
        {
            var d = Vector2.Distance(Position, point);
            return d < Radius;
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            base.OnMouseEvent(me);
            if (!HasMouseFocus) return;
            if (me is MouseWheelUp) Radius += 1.5f;
            if (me is MouseWheelDown) Radius -= 1.5f;
        }

        public override void Draw() => DrawCircleV(Position, Radius, Color);
    }

    public class Rectangle : UIComponent
    {
        public Vector2 Size { get; set; }

        public override bool ContainsPoint(Vector2 point)
        {
            return point.X > Position.X && point.X < Position.X + Size.X &&
                 point.Y > Position.Y && point.Y < Position.Y + Size.Y;
        }

        public override void Draw() => DrawRectangleV(Position, Size, Color);
    }


    public class Polygon : UIComponent
    {
        //points are ordered anti-clockwise and are drawn relative to Position, i.e. as if it were 0,0
        //in addition Position acts as the 1st point and opens/closes the polygon
        public Vector2[] Points { get; init; }
        public Vector2[] TextCoords { get; init; }

        private static Texture2D texture2D = new() { id = 1, };

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }

        public override void Draw() => DrawTexturePoly(texture2D, Position, Points, TextCoords, Points.Length, Color);
    }
}

