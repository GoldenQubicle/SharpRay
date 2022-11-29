namespace GardenOfCards
{
    internal class CardSlot : Entity, IHasRender, IHasCollider
    {
        public const string HandTag = "HandSlot";
        public static float LineWidth = 2f;
        public ICollider Collider { get; }
        public bool IsOccupied => CurrentCard != Game.BlankCard;
        public Card CurrentCard { get; private set; } 
        public bool IsTargeted;


        public CardSlot(Card card) : this(card.Position, HandTag)
        {
            CurrentCard = card;
        }

        public CardSlot(Vector2 position, string tag)
        {
            Size = new(Card.Width, Card.Height);
            Position = position;
            Collider = new RectCollider { Position = Position, Size = Size };
            CurrentCard = Game.BlankCard;
            RenderLayer = 1;
            Tag = tag;
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
                    CurrentCard = Game.BlankCard;
                }
            }
        }

        public override void Render()
        {
            var color = IsOccupied ? Color.RED : Color.GREEN;

            DrawRectangleRoundedLines((Collider as RectCollider).Rect, Card.Roundness, 8, LineWidth, color);

            //Collider.Render();
        }
    }
}
