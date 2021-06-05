using Raylib_cs;
using System;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Serialization;

namespace ProtoCity
{
    public abstract class UIComponent
    {
        private int _id;
        public int Id
        {
            get => _id;
            init => _id = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Id), $"{typeof(UIComponent).Name} Id cannot be negative");
        }
        public Vector2 Position { get; set; }
        public Color BaseColor { get; set; }
        public Color HighLightColor { get; set; } = Color.BROWN;
        public Color Color { get; protected set; }
        public bool HasMouseFocus { get; protected set; }
        private bool IsDragged { get; set; }

        private Vector2 DragStart;
        private Vector2 DragOffSet;
        public abstract bool ContainsPoint(Vector2 point);

        public Action<IEditEvent> OnEdit = e => Program.UIEventHandler(e);
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
                DragStart = me.Position;
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
    }

    public class Rectangle : UIComponent
    {
        public Vector2 Size { get; set; }

        public override bool ContainsPoint(Vector2 point)
        {
            return point.X > Position.X && point.X < Position.X + Size.X &&
                 point.Y > Position.Y && point.Y < Position.Y + Size.Y;
        }
    }

    public class Polygon : UIComponent
    {
        //points are ordered anti-clockwise and are drawn relative to Position, i.e. as if it were 0,0
        //in addition Position acts as the 1st point and opens/closes the polygon
        public Vector2[] Points { get; init; }
        public Vector2[] TextCoords { get; init; }

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }
    }
}

