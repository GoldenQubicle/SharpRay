namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        public static void OnTurnStart(TurnData turnData)
        {
            for (var i = 0; i < turnData.HandSize; i++)
            {
                var card = new Card(GetCardPosition(i, turnData.HandSize));
                var cardSlot = new CardSlot(card);
                var cardSlotEmpty = new CardSlot(cardSlot.Position + new Vector2(0, -256));
                AddEntity(card);
                AddEntity(cardSlot);
                AddEntity(cardSlotEmpty);
            }

        }

        private static Vector2 GetCardPosition(int idx, int handSize)
        {
            var totalWidth = handSize * Card.Width + handSize * Card.Margin + Card.Margin;
            var relativeXPos = idx * Card.Width + idx * Card.Margin + Card.Margin;
            return new(relativeXPos + (Game.WindowWidth - totalWidth) /2, Game.WindowHeight / 2);
        }

        public static void OnTurnEnd()
        {

        }
    }
}
