using Rectangle = Raylib_cs.Rectangle;

namespace GardenOfCards
{
    internal class Card : DragEditShape
    {
        internal const int Width = 128;
        internal const int Height = 192;

        public Rectangle Rectangle { get; private set; }

        public Card(Vector2 position)
        {
            Size = new(Width, Height);
            Position = position;
            Rectangle = new(Position.X, Position.Y, Size.X, Size.Y);
        }

        public override bool ContainsPoint(Vector2 point) => CheckCollisionPointRec(point, Rectangle);



        public override void Update(double deltaTime)
        {
            Rectangle = new(Position.X, Position.Y, Size.X, Size.Y);
        }

        public override void Render()
        {
            base.Render();
            DrawRectangleRounded(Rectangle, .25f, 8, ColorRender);
        }
    }
}
