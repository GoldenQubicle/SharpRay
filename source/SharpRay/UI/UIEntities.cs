﻿using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace SharpRay
{

    public sealed class ImageTexture : UIEntity
    {
        public Texture2D Texture2D { get; private set; }
        public Color Color { get; init; }

        public ImageTexture(Image image, Color color)
        {
            Color = color;
            Texture2D = LoadTextureFromImage(image);
            UnloadImage(image);
        }

        public override void Render()
        {
            DrawTexture(Texture2D, (int)Position.X, (int)Position.Y, Color);
        }
    }

    public class Label : UIEntity
    {
        public Label() { }
        public string Text { get; set; }
        public Font Font { get; init; }
        public Color TextColor { get; init; }
        public Color FillColor { get; set; }
        public Raylib_cs.Rectangle Rectangle
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X + Margins.X,
                y = Position.Y + Margins.Y,
                width = Size.X - Margins.X,
                height = Size.Y - Margins.Y
            };
        }
        public float FontSize { get; init; } = 15f;
        public float Spacing { get; init; } = 1f;
        public bool WordWrap { get; init; } = false;
        public Vector2 Margins { get; init; }

        public override void Render()
        {
            DrawRectangleV(Position, Size, FillColor);
            DrawRectangleLines((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, TextColor);
            DrawTextRec(GetFontDefault(), Text, Rectangle, FontSize, Spacing, WordWrap, TextColor);
        }
    }

    public sealed class Button : Label
    {
        public Color FocusColor { get; init; }
        public Color BaseColor { get; init; }
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
            if (HasMouseFocus && e is MouseLeftClick)
                EmitEvent(OnMouseLeftClick(this));
        }
    }



    public abstract class DragEditShape : UIEntity
    {
        //not too sure about these 2...          
        public Action<DragEditShape> OnRightMouseClick { get; set; }
        public Color ColorDefault { get; set; }
        public Color ColorFocused { get; set; }
        protected Color ColorRender { get; set; }

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

        public override void Render()
        {
            ColorRender = HasMouseFocus ? ColorFocused : ColorDefault;
        }
    }

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


    public sealed class Polygon : DragEditShape
    {
        //points are ordered anti-clockwise and are drawn relative to Position, i.e. as if Position were 0,0
        //if the points array is not closed, position will act as the 1st point and opens/closes the polygon
        public Vector2[] Points { get; init; }
        public Vector2[] TextCoords { get; init; }

        private static Texture2D texture2D = new() { id = 1, }; // seems like textures need to be unloaded btw

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }

        public override void Render()
        {
            base.Render();
            DrawTexturePoly(texture2D, Position, Points, TextCoords, Points.Length, ColorRender);
        }
        public static Vector2[] CreatePoints(int sides, float size)
        {
            var points = new Vector2[sides + 1];

            for (var i = 0; i <= sides; i++)
            {
                var t = i * MathF.Tau / sides;
                points[i] = new Vector2
                {
                    X = MathF.Sin(t) * size,
                    Y = MathF.Cos(t) * size
                };
            }
            return points;
        }
    }
}

