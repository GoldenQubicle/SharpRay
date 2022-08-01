namespace BreakOut
{
    public class Brick: Entity, IHasRender, IHasCollider
    {
        public enum Type
        {
            Simple,
        }

        public ICollider Collider { get; }

        public Brick(Vector2 position)
        {
            Size = new Vector2(100, 20);
            Position = position;
            Collider = new RectCollider
            {
                Position = Position,
                Size = Size
            };
        }

        public override void Render()
        {
            DrawRectangleGradientEx((Collider as RectCollider).Rect, Color.YELLOW, Color.PURPLE, Color.ORANGE, Color.LIME);
            //Collider.Render();
        }
    }
}
