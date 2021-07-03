using Raylib_cs;
using static Raylib_cs.Raylib;
using SharpRay.Entities;
using System.Numerics;

namespace SharpRay.Gui
{
    public class Label : GuiEntity
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
}

