namespace GardenOfCards
{
    internal class CardSlot : Entity, IHasRender, IHasCollider
    {
        public static float LineWidth = 2f;
        public ICollider Collider { get; }
        public bool IsOccupied => CurrentCard != null;
        public Card CurrentCard { get; set; }
        public bool IsTargeted;
        public CardSlot(Card card) : this(card.Position)
        {
            CurrentCard = card;
        }

        public CardSlot(Vector2 position)
        {
            Size = new(Card.Width, Card.Height);
            Position = position;
            Collider = new RectCollider { Position = Position, Size = Size };

            RenderLayer = 1;
        }

        public void SetCurrentCard(Card card)
        {
            CurrentCard = card;
        } 

        public override void Update(double deltaTime)
        {
            if (IsOccupied)
            {
                if (!Collider.Overlaps(CurrentCard.Collider))
                {
                    CurrentCard = null;
                }
            }
        }

        public override void Render()
        {
            var color = IsTargeted ? Color.RED : Color.GREEN;

            DrawRectangleRoundedLines((Collider as RectCollider).Rect, Card.Roundness, 8, LineWidth, Color.DARKBROWN);

            //Collider.Render();
        }
    }
}
