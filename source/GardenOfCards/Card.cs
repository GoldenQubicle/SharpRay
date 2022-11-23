using Rectangle = Raylib_cs.Rectangle;

namespace GardenOfCards
{
    internal class Card : DragEditShape
    {
        internal const int Width = 128;
        internal const int Height = 192;

        public Rectangle Rectangle { get; private set; }
        private Rectangle RectangleSlot { get; set; }

        private (Vector2 start, Vector2 end) _easingData;
        private bool _doEasing;

        private readonly Easing _easing = new(Easings.EaseCubicInOut, 350);

        public Card(Vector2 position)
        {
            Size = new(Width, Height);
            Position = position;
            Rectangle = new(Position.X, Position.Y, Size.X, Size.Y);
            RectangleSlot = new(Position.X, Position.Y, Size.X, Size.Y);
        }

        public override void Render()
        {
            base.Render();
            DrawRectangleRounded(Rectangle, .25f, 8, ColorRender);
            DrawRectangleRoundedLines(RectangleSlot, .25f, 8, 2f, Color.DARKBROWN);
        }

        public override void Update(double deltaTime)
        {
            Rectangle = new(Position.X, Position.Y, Size.X, Size.Y);

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
            _easingData = (new(Position.X, Position.Y), new(RectangleSlot.x, RectangleSlot.y));
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
                Position = new(RectangleSlot.x, RectangleSlot.y);
            }
        }

        public override bool ContainsPoint(Vector2 point) => CheckCollisionPointRec(point, Rectangle);
    }
}
