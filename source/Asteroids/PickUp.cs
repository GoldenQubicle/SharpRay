namespace Asteroids
{
    public class PickUp : Entity, IHasCollider, IHasRender, IHasUpdate
    {
        public ICollider Collider { get; }

        public Action<Ship> OnPickUp { get; init; }

        public PickUp()
        {
            Size = new Vector2(25, 25);
            Collider = new RectCollider
            {
                Size = Size
            };

            RenderLayer = RlPickUps;
        }

        public override void Update(double deltaTime)
        {

        }

        public override void Render()
        {
             DrawRectanglePro((Collider as RectCollider).Rect, Position, 0f, Color.YELLOW);
        }
    }
}
