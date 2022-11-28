namespace GardenOfCards
{
    internal class Card : DragEditShape, IHasCollider, IHasCollision
    {
        public static Card Blank = new();

        internal const int Width = 96;
        internal const int Height = 144;
        internal const int Margin = 30;
        internal const float Roundness = .25f;
        public ICollider Collider { get; }
        public Vector2 EasingTarget { get; set; }
        public string Text { get; }

        private (Vector2 start, Vector2 end) _easingData;
        private bool _doEasing;

        private readonly Easing _easing = new(Easings.EaseCubicInOut, 200);

        public Card() { }

        public Card(Vector2 position, string text)
        {
            Size = new(Width, Height);
            Position = position;
            EasingTarget = Position;

            Collider = new RectCollider { Position = Position, Size = Size };

            ColorDefault = Color.WHITE;
            ColorFocused = Color.RED;

            RenderLayer = 2;
            Text = text;
        }

        public override void Render()
        {
            base.Render();
            DrawRectangleRounded((Collider as RectCollider).Rect, Roundness, 8, ColorRender);
            DrawTextV(Text, Position, 12, Color.PURPLE);
            //Collider.Render();
        }

        public override void Update(double deltaTime)
        {
            (Collider as RectCollider).Position = Position;
            DoEasing(deltaTime);
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (_doEasing) return;

            base.OnMouseEvent(e);

            if (e is MouseLeftRelease mrl) StartEasing();
        }

        private void StartEasing()
        {
            _easing.Reset();
            _easingData = (new(Position.X, Position.Y), EasingTarget);
            _doEasing = true;
            HasMouseFocus = false;
        }

        private void DoEasing(double deltaTime)
        {
            if (!_doEasing) return;

            Position = Vector2.Lerp(_easingData.start, _easingData.end, _easing.GetValue());
            _easing.Update(deltaTime);

            if (_easing.IsDone())
            {
                _doEasing = false;
                Position = _easingData.end;
            }
        }

        public override bool ContainsPoint(Vector2 point) => Collider.ContainsPoint(point);

        public void OnCollision(IHasCollider e)
        {
            if (e is CardSlot { IsOccupied: false } cs)
            {
                EasingTarget = cs.Position;
                cs.SetCurrentCard(this);
                cs.IsTargeted = true;
            }
        }
    }
}
