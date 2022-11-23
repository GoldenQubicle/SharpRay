namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        public static void OnTurnStart(TurnData turnData)
        {
            var offset = (Game.WindowWidth - Game.GetWidthForNCards(turnData.HandSize)) / 2;

            for (var i = 0; i < turnData.HandSize; i++)
            {
                var pos = Game.GetCardPosition(i, turnData.HandSize) + new Vector2((int)offset, Game.WindowHeight - Card.Height - Card.Margin - CardSlot.LineWidth);
                var card = new Card(pos);
                var cardSlot = new CardSlot(card);
                var cardSlotEmpty = new CardSlot(cardSlot.Position + new Vector2(0, -256));
                AddEntity(card);
                AddEntity(cardSlot);
                //AddEntity(cardSlotEmpty);
            }

        }



        public static void OnTurnEnd()
        {

        }
    }
}
