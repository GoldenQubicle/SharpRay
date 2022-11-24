namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        public static void OnGameStart()
        {
            CreatePlant(3);
        }

        public static void OnTurnStart(TurnData turnData)
        {
            var offset = (Game.WindowWidth - Game.GetWidthForNCards(turnData.HandSize)) / 2;

            for (var i = 0; i < turnData.HandSize; i++)
            {
                var pos = Game.GetCardPosition(i, turnData.HandSize) + new Vector2((int)offset, Game.WindowHeight - Card.Height - Card.Margin - CardSlot.LineWidth);
                var card = new Card(pos);
                var cardSlot = new CardSlot(card);
                AddEntity(card);
                AddEntity(cardSlot);
            }
        }

        public static void OnTurnEnd()
        {

        }

        private static void CreatePlant(int startCards)
        {
            var potData = GetPotRenderData(startCards);
            var plantPosition = new Vector2((Game.WindowWidth - potData.Width) / 2, Game.WindowHeight * .2f);
            potData = potData.ApplyOffset(plantPosition);

            for (var i = 0; i < startCards; i++)
            {
                var pos = Game.GetCardPosition(i, startCards) + potData.SlotOffset;
                AddEntity(new CardSlot(pos));
            }

            AddEntity(new Plant(plantPosition, potData));
        }

        private static PotRenderData GetPotRenderData(int nSlots)
        {
            var basinWidth = Game.GetWidthForNCards(nSlots);
            var basinHeight = Card.Height * 1.75f;
            var basinSlant = 30;
            var basinThickness = 5;
            var rimWidth = basinWidth + 4 * basinSlant;
            var rimThickness = 50;
            var offsetBasin = new Vector2(basinSlant, rimThickness);

            return new
            (
                Height: basinHeight + basinThickness / 2 + rimThickness,
                Width: rimWidth,
                RimStart: new(0, rimThickness / 2),
                RimEnd: new(rimWidth, rimThickness / 2),
                RimThickness: rimThickness,
                RimColor: Color.DARKBROWN,
                BasinLeftUp: offsetBasin,
                BasinLeftDown: offsetBasin + new Vector2(basinSlant, basinHeight),
                BasinRightDown: offsetBasin + new Vector2(basinWidth + basinSlant, basinHeight),
                BasinRightUp: offsetBasin + new Vector2(basinWidth + 2 * basinSlant, 0),
                BasinThickness: basinThickness,
                BasinColor: Color.DARKBROWN,
                SlotOffset: new((rimWidth - basinWidth) / 2, rimThickness + (basinHeight - Card.Height) / 2)
            );
        }
    }
}
