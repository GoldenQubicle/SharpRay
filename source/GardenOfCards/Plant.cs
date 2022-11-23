namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate, IHasCollider, IHasCollision
    {
        public ICollider Collider { get; }
        public Plant()
        {
            Collider = new RectCollider
            {
                Position = new(Card.Margin, Card.Margin), 
                Size = new(Card.Width, Card.Height)
            };
        }

        public override void Render()
        {
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .25f, 8, 2f, Color.DARKBROWN);
        }

        public override void Update(double deltaTime)
        {

        }


        public void OnCollision(IHasCollider e)
        {
            if (e is Card c)
            {

            }
        }
    }
}
