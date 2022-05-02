using Raylib_cs;
using static Raylib_cs.Raylib;
using SharpRay.Entities;
using System.Numerics;

namespace SharpRay.Gui
{
    /// <summary>
    /// A simple text label. Postion is the center from which the label is drawn. 
    /// </summary>
    public class Label : GuiEntity
    {
        public Label() { }
        public string Text { get; set; }
        public Font Font { get; init; } = GetFontDefault();
        public Color TextColor { get; set; } = Color.WHITE;
        public Color FillColor { get; set; } = Color.LIGHTGRAY;

        public Raylib_cs.Rectangle Rectangle
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X - Size.X / 2 + Margins.X,
                y = Position.Y - Size.Y / 2 + Margins.Y,
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
            var offset = Position - Size / 2;
            DrawRectangleV(offset, Size, FillColor);
            DrawRectangleLines((int)offset.X, (int)offset.Y, (int)Size.X, (int)Size.Y, TextColor);
            DrawTextRec(Font, Text, Rectangle, FontSize, Spacing, WordWrap, TextColor);
        }
    }
}

