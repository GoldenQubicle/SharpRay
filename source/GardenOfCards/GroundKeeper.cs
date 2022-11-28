namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        private static readonly Dictionary<Plant, List<CardSlot>> Plants = new();

        public static void OnGameStart()
        {
            CreatePlant(new PotData());
            OnTurnStart(new(Turn: 1, HandSize: 4));


        }

        public static void OnTurnStart(TurnData turnData)
        {
            var offset = (Game.WindowWidth - Game.GetWidthForNCards(turnData.HandSize)) / 2;

            for (var i = 0; i < turnData.HandSize; i++)
            {
                var pos = Game.GetCardPosition(i, turnData.HandSize) + new Vector2((int)offset, Game.WindowHeight - Card.Height - Card.Margin - CardSlot.LineWidth);
                var card = new Card(pos, i.ToString());
                var cardSlot = new CardSlot(card);
                AddEntity(card);
                AddEntity(cardSlot);
            }
        }

        public static void OnTurnEnd()
        {
            foreach(var kvp in Plants)
            {
                var cards = kvp.Value.Where(cs => cs.IsOccupied).Select(cs => cs.CurrentCard).ToList();
                kvp.Value.ForEach(cs => cs.SetCurrentCard(null));
            }

            RemoveEntitiesOfType<Card>();
        }

        private static void CreatePlant(PotData data)
        {
            var potData = GetPotRenderData(data);
            var plantPosition = new Vector2((Game.WindowWidth - potData.Width) / 2, Game.WindowHeight * .2f);
            potData = potData.ApplyOffset(plantPosition);
            var plant = new Plant(plantPosition, potData);
            Plants.Add(plant, new List<CardSlot>());

            for (var i = 0; i < data.nSlots; i++)
            {
                var pos = Game.GetCardPosition(i, data.nSlots) + potData.SlotOffset;
                Plants[plant].Add(new CardSlot(pos));
            }

            AddEntity(plant);
            Plants[plant].ForEach(AddEntity);
        }

        private static PotRenderData GetPotRenderData(PotData data)
        {
            var basinWidth = Game.GetWidthForNCards(data.nSlots);
            var basinHeight = Card.Height * data.BasinHeightFactor;
            var rimWidth = basinWidth + 4 * data.BasinSlant;
            var offsetBasin = new Vector2(data.BasinSlant, data.RimThickness);

            //TODO factor out basin & rim color to serializable pot data
            return new
            (
                Height: basinHeight + data.BasinThickness / 2 + data.RimThickness,
                Width: rimWidth,
                RimStart: new(0, data.RimThickness / 2),
                RimEnd: new(rimWidth, data.RimThickness / 2),
                RimThickness: data.RimThickness,
                RimColor: Color.DARKBROWN,
                BasinLeftUp: offsetBasin,
                BasinLeftDown: offsetBasin + new Vector2(data.BasinSlant, basinHeight),
                BasinRightDown: offsetBasin + new Vector2(basinWidth + data.BasinSlant, basinHeight),
                BasinRightUp: offsetBasin + new Vector2(basinWidth + 2 * data.BasinSlant, 0),
                BasinThickness: data.BasinThickness,
                BasinColor: Color.DARKBROWN,
                SlotOffset: new((rimWidth - basinWidth) / 2, data.RimThickness + (basinHeight - Card.Height) / 2)
            );
        }
    }
}
