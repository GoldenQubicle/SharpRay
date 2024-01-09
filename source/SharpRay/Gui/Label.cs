namespace SharpRay.Gui
{
	/// <summary>
	/// A simple text label. Postion is the center from which the label is drawn. 
	/// </summary>
	public class Label : GuiEntity
	{
		public string Text { get; set; }
		public Font Font { get; init; } = GetFontDefault( );
		public Color TextColor { get; set; } = WHITE;
		public Color OutlineColor { get; set; } = WHITE;
		public Color FillColor { get; set; } = LIGHTGRAY;
		public bool DoCenterText { get; set; }
		public float FontSize { get; init; } = 15f;
		public float Spacing { get; init; } = 1f;
		public Vector2 TextOffSet { get; set; }
		public bool HasOutlines { get; init; } = true;
		public Action<Label> UpdateAction { get; init; }
		public double TriggerTime { get; init; }
		public double ELapsedTime { get; private set; }

		public override void Render()
		{
			if (!IsVisible)
				return;

			var offset = Position - Size / 2;
			DrawRectangleV(offset, Size, FillColor);

			if (HasOutlines)
				DrawRectangleLines((int)offset.X, (int)offset.Y, (int)Size.X, (int)Size.Y, OutlineColor);

			if (DoCenterText)
			{
				var textSize = MeasureTextEx(Font, Text, FontSize, Spacing);
				TextOffSet = (Size - textSize) / 2;
			}

			var textPos = new Vector2(Position.X - Size.X / 2 + TextOffSet.X, Position.Y - Size.Y / 2 + TextOffSet.Y);
			DrawTextEx(Font, Text, textPos, FontSize, Spacing, TextColor);
		}

		public override void Update(double deltaTime)
		{
			if (!IsVisible)
				return;

			ELapsedTime += deltaTime;
			UpdateAction?.Invoke(this);
		}
	}
}

