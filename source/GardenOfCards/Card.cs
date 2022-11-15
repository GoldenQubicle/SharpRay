using Rectangle = Raylib_cs.Rectangle;

namespace GardenOfCards
{
    internal class Card : DragEditShape
    {
        private Rectangle _rectangle;


        public override bool ContainsPoint(Vector2 point) => CheckCollisionPointRec(point, _rectangle);
        

        public override void Update(double deltaTime)
        {
            _rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }

        public override void Render()
        {
            base.Render();
            DrawRectangleRounded(_rectangle, .25f, 8, ColorRender);
        }



    }
}
