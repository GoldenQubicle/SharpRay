namespace SharpRay.Gui
{
    /// <summary>
    /// A simple text label. Postion is the center from which the label is drawn. 
    /// </summary>
    public class Label : GuiEntity
    {
        public string Text { get; set; }
        public Font Font { get; init; } = GetFontDefault();
        public Color TextColor { get; set; } = WHITE;
        public Color FillColor { get; set; } = LIGHTGRAY;
        public bool DoCenterText { get; set; }

        public Raylib_cs.Rectangle Rectangle
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X - Size.X / 2 + TextOffSet.X,
                y = Position.Y - Size.Y / 2 + TextOffSet.Y,
                width = Size.X - TextOffSet.X,
                height = Size.Y - TextOffSet.Y
            };
        }
        public float FontSize { get; init; } = 15f;
        public float Spacing { get; init; } = 1f;
        public bool WordWrap { get; init; } = false;
        public Vector2 TextOffSet { get; set; }
        public bool HasOutlines { get; init; } = true;
        public Action<Label> UpdateAction { get; init; }
        public double TriggerTime { get; init; }
        public double ELapsedTime { get; private set; }

        public override void Render()
        {
            var offset = Position - Size / 2;
            DrawRectangleV(offset, Size, FillColor);

            if (HasOutlines)
                DrawRectangleLines((int)offset.X, (int)offset.Y, (int)Size.X, (int)Size.Y, TextColor);

            if (DoCenterText)
            {
                var textSize = MeasureTextEx(Font, Text, FontSize, Spacing);
                TextOffSet = (Size - textSize) / 2;
            }

            var textPos = new Vector2(Position.X - Size.X / 2 + TextOffSet.X, Position.Y - Size.Y / 2 + TextOffSet.Y);

            if (WordWrap)
            {
                var fitted = Text;
                var remaining = new List<char>();
                var size = MeasureTextEx(Font, fitted, FontSize, Spacing);
                while (size.X > Rectangle.width)
                {
                    remaining.Add(fitted.TakeLast(1).First());
                    fitted = fitted[0..(fitted.Length-1)];
                    size = MeasureTextEx(Font, fitted, FontSize, Spacing);
                }
                remaining.Reverse();
                var toBeFitted = new string(remaining.ToArray());
            }

            DrawTextEx(Font, Text, textPos, FontSize, Spacing, TextColor);

            //DrawTextPr(Text, (int)Rectangle.x, (int)Rectangle.y, (int) FontSize, TextColor);
            //DrawTextRec(Font, Text, Rectangle, FontSize, Spacing, WordWrap, TextColor);

        }

        public override void Update(double deltaTime)
        {
            ELapsedTime += deltaTime;
            UpdateAction?.Invoke(this);
        }
    }
}

