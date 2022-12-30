namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        private static TurnData currentTurn;

        public static void OnGameStart(GameStartData data)
        {
            CreatePlant(data.Pot);
            OnTurnStart(data.Turn);
        }

        public static void OnTurnStart(TurnData turnData)
        {
            var offset = (Game.WindowWidth - Game.GetWidthForNCards(turnData.HandSize)) / 2;
            var suites = Enum.GetValues<Suite>().Except(new []{Suite.Seed}).ToArray();
            for (var i = 0; i < turnData.HandSize; i++)
            {
                var suiteData = GetSuiteRenderData(suites[GetRandomValue(0, suites.Length - 1)]);
                var pos = Game.GetCardPosition(i) + new Vector2((int)offset, Game.WindowHeight - Card.Height - Card.Margin - CardSlot.LineWidth);
                var card = new Card(pos, suiteData);
                var cardSlot = new CardSlot(card);
                AddEntity(card);
                AddEntity(cardSlot);
            }

            currentTurn = turnData;
        }

        public static void OnTurnEnd()
        {
            foreach (var plant in GetEntities<Plant>())
            {
                var cards = GetEntitiesByTag<CardSlot>(plant.Tag)
                    .Where(cs => cs.IsOccupied) // could even just get all cards and penalizing leaving slots blank..?
                    .Select(cs => cs.CurrentCard);

                plant.OnTurnEnd(cards);
            }

            RemoveEntitiesOfType<Card>();
            RemoveEntitiesOfType<CardSlot>(cs => cs.Tag.Equals(CardSlot.HandTag));

            AddEventAction(() => GetEntities<CardSlot>().ToList().ForEach(cs => cs.SetCurrentCard(Game.BlankCard)));

            //TODO Generate & apply Adversities 

            OnTurnStart(new TurnData(currentTurn.Turn + 1, currentTurn.HandSize));
        }

        private static void CreatePlant(PotData data)
        {
           
            var potData = GetPotRenderData(data);
            var plantPosition = new Vector2((Game.WindowWidth - potData.Width) / 2, Game.WindowHeight * .65f);
            potData = potData.ApplyOffset(plantPosition);
            var soilData = GetSoilRenderData(potData, plantPosition);
            var seed = GetSuiteRenderData(Suite.Seed);

            for (var i = 0; i < data.nSlots; i++)
            {
                var pos = Game.GetCardPosition(i) + potData.SlotOffset;
                if (i == 0)
                {
                    var seedCard = new Card(pos, seed);
                    var slot = new CardSlot(seedCard, seed.Suite.ToString());
                    AddEntity(seedCard);
                    AddEntity(slot);
                }
                else
                {
                    AddEntity(new CardSlot(pos, seed.Suite.ToString()));
                }
                
            }

            var plant = new Plant(plantPosition, potData, soilData, seed.Suite.ToString());
            AddEntity(plant);
        }

        private static SuiteData GetSuiteRenderData(Suite suite) => suite switch
        {
            Suite.Seed => new(Suite.Seed, GetRandomStat(), Color.BEIGE, Color.BROWN),
            Suite.Water => new(Suite.Water, GetRandomStat(), Color.SKYBLUE, Color.BLUE),
            Suite.Light => new(Suite.Light, GetRandomStat(), Color.YELLOW, Color.GOLD),
            Suite.Nutrient => new(Suite.Nutrient, GetRandomStat(), Color.GREEN, Color.LIME),
            _ => throw new ArgumentOutOfRangeException(nameof(suite), suite, null)
        };

        private static int GetRandomStat() => GetRandomValue(1, 9);

        private static SoilRenderData GetSoilRenderData(PotRenderData potData, Vector2 plantPosition)
        {
            var uv = new Vector2[5];
            var center = new Vector2(potData.Width / 2, potData.Height / 2) + plantPosition;
            var points = new[] {
                potData.BasinLeftUp - center,
                potData.BasinLeftDown - center,
                potData.BasinRightDown - center,
                potData.BasinRightUp - center,
                potData.BasinLeftUp - center
            };
            return new(center, points, uv, new Texture2D { id = 1 }, Color.BROWN);
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
                SlotOffset: new((rimWidth - basinWidth) / 2, data.RimThickness + (basinHeight - Card.Height - CardSlot.LineWidth * 2) / 2)
            );
        }
    }
}
