namespace GardenOfCards
{
	internal class Card : DragEditShape, IHasCollider, IHasCollision
	{
		public ICollider Collider { get; }
		public Vector2 EasingTarget { get; set; }
		public Suite Suite => _suiteData.Suite;
		public int Stat => _suiteData.Number;

		internal const int Width = 64;
		internal const int Height = 96;
		internal const int Margin = 30;
		internal const float Roundness = .25f;

		private (Vector2 start, Vector2 end) _easingData;
		private bool _doEasing;
		private readonly Easing _easing = new(Easings.EaseCubicInOut, 200);
		private SuiteData _suiteData;

		public Card()
		{
			Collider = new RectCollider( );
		}

		public Card(Vector2 position, SuiteData data)
		{
			_suiteData = data;
			Size = new(Width, Height);
			Position = position;
			EasingTarget = Position;

			Collider = new RectCollider { Position = Position, Size = Size };

			ColorDefault = Color.White;
			ColorFocused = Color.Red;

			RenderLayer = 2;
		}



		public override void Render()
		{
			base.Render( );
			DrawRectangleRounded(( Collider as RectCollider ).Rect, Roundness, 8, _suiteData.colors.RenderColor);

			var numberSize = MeasureTextEx(GetFontDefault( ), _suiteData.Number.ToString( ), Height, 1);
			var numberPos = Position + Size / 2 - numberSize / 2 + new Vector2(0, 8);
			DrawTextV(_suiteData.Number.ToString( ), numberPos, Height, _suiteData.colors.TextColor);

			//Collider.Render();
		}

		public override void Update(double deltaTime)
		{
			Collider.Position = Position;
			DoEasing(deltaTime);
		}

		public override void OnMouseEvent(IMouseEvent e)
		{
			if (_doEasing)
				return;

			base.OnMouseEvent(e);

			if (e is MouseLeftRelease mrl)
				StartEasing( );
		}

		private void StartEasing()
		{
			_easing.Reset( );
			_easingData = (new(Position.X, Position.Y), EasingTarget);
			_doEasing = true;
			HasMouseFocus = false;
		}

		private void DoEasing(double deltaTime)
		{
			if (!_doEasing)
				return;

			Position = Vector2.Lerp(_easingData.start, _easingData.end, _easing.GetValue( ));
			_easing.Update(deltaTime);

			if (!_easing.IsDone( ))
				return;

			_doEasing = false;
			Position = _easingData.end;
		}

		public override bool ContainsPoint(Vector2 point) => Collider.ContainsPoint(point);

		public void OnCollision(IHasCollider e)
		{
			if (e is CardSlot { IsOccupied: false } cs)
			{
				EasingTarget = cs.Position;
				cs.SetCurrentCard(this);
			}
		}
	}
}
