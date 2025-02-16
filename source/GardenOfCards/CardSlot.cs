namespace GardenOfCards
{
    internal class CardSlot : Entity, IHasRender, IHasCollider
    {
        public const string HandTag = "HandSlot";
        public static float LineWidth = 2f;
        public ICollider Collider { get; }
        public bool IsOccupied => CurrentCard != Game.BlankCard;
        public Card CurrentCard { get; private set; }
        public int Idx { get; }

        public CardSlot(Card card, int idx) : this(card.Position, HandTag, idx)
        {
            CurrentCard = card;
        }
        public CardSlot(Vector2 pos, string tag, int idx)
        {
            Size = new(Card.Width, Card.Height);
            Position = pos;
            Collider = new RectCollider { Position = Position, Size = Size };
            CurrentCard = Game.BlankCard;
            RenderLayer = 1;
            Tag = tag;
            Idx = idx;
        }

        public void SetCurrentCard(Card card) => CurrentCard = card;

        public override void Update(double deltaTime)
        {
	        if (!IsOccupied) return;

	        if (!Collider.Overlaps(CurrentCard.Collider))
	        {
		        CurrentCard = Game.BlankCard;
	        }
        }

        public override void Render()
        {
            var color = IsOccupied ? Color.DarkGray : Color.LightGray;

            DrawRectangleRoundedLines((Collider as RectCollider).Rect, Card.Roundness, 8, color);

            //Collider.Render();
        }
    }
}
