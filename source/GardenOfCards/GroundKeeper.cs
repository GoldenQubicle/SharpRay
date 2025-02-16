namespace GardenOfCards
{
    internal static class GroundKeeper
    {
        public static HashSet<(Suite, int)> AlreadyDealt { get; private set; }

        public static TurnData CurrentTurn { get; private set; }


        public static void OnGameStart(GameStartData data)
        {
            CreatePlant(data.Pot);
            OnTurnStart(data.Turn);
        }

        public static void OnTurnStart(TurnData turnData)
        {
            CurrentTurn = turnData;
            AddCardsToHandSlots(DealHand(isNewTurn: true));
        }

        public static void OnDealHand()
        {
            if (AlreadyDealt.Count + CurrentTurn.HandSize >= Game.Suites.Length * Game.MaxStat)
                return;

            var hand = DealHand();

            foreach (var cardSlot in GetEntitiesByTag<CardSlot>(CardSlot.HandTag))
            {
                if (cardSlot.IsOccupied)
                {
                    var oldCard = cardSlot.CurrentCard;
                    cardSlot.SetCurrentCard(Game.BlankCard);
                    RemoveEntity(oldCard);
                }

                var newCard = CreateCard(hand[cardSlot.Idx], cardSlot.Position);
                cardSlot.SetCurrentCard(newCard);
                AddEntity(newCard);
            }

            CurrentTurn = CurrentTurn with { HandsDealt = CurrentTurn.HandsDealt + 1 };
        }

        public static void OnTurnEnd()
        {
            foreach (var plant in GetEntities<Plant>())
            {
                var cards = GetEntitiesByTag<CardSlot>(plant.Tag)
                    .Where(cs => cs.IsOccupied) // could even just get all cards and penalizing leaving slots blank..?
                    .Select(cs => cs.CurrentCard).ToList();

                plant.OnTurnEnd(cards);
            }

            RemoveEntitiesOfType<Card>();
            RemoveEntitiesOfType<CardSlot>(cs => cs.Tag.Equals(CardSlot.HandTag));

            AddEventAction(() => GetEntities<CardSlot>().ToList().ForEach(cs => cs.SetCurrentCard(Game.BlankCard)));

            //TODO Generate & apply Adversities 



            OnTurnStart(CurrentTurn with { Number = CurrentTurn.Number + 1, HandsDealt = 0 });
        }

        private static void AddCardsToHandSlots(List<(Suite suite, int stat)> hand)
        {
            var offset = GetHandSlotOffSet(CurrentTurn.HandSize);

            for (var idx = 0; idx < CurrentTurn.HandSize; idx++)
            {
                var pos = Game.GetCardPosition(idx) + offset;
                var card = CreateCard(hand[idx], pos);
                var cardSlot = new CardSlot(card, idx);
                AddEntity(card);
                AddEntity(cardSlot);
            }
        }

        private static Card CreateCard((Suite suite, int stat) data, Vector2 position) =>
            new(position, GetSuiteRenderData(data));

        private static Vector2 GetHandSlotOffSet(int handSize)
        {
            var offsetX = (Game.WindowWidth - Game.GetWidthForNCards(handSize)) / 2;
            var offSetY = Game.WindowHeight - Card.Height - Card.Margin - CardSlot.LineWidth;
            var offsetV = new Vector2((int)offsetX, (int)offSetY);
            return offsetV;
        }

        private static List<(Suite suite, int stat)> DealHand(bool isNewTurn = false)
        {
            if (isNewTurn)
                AlreadyDealt = new();

            var hand = new List<(Suite, int)>();

            while (hand.Count < CurrentTurn.HandSize)
            {
                var card = (GetRandomSuite(), GetRandomStat());

                if (AlreadyDealt.Contains(card))
                    continue;

                hand.Add(card);
                AlreadyDealt.Add(card);
            }

            return hand;
        }

        private static void CreatePlant(PotData data)
        {
            var potData = GetPotRenderData(data);
            var plantPosition = new Vector2((Game.WindowWidth - potData.Width) / 2, Game.WindowHeight * .65f);
            potData = potData.ApplyOffset(plantPosition);
            var soilData = GetSoilRenderData(potData, plantPosition);
            var seed = GetSuiteRenderData((Suite.Seed, GetRandomStat()));

            var needs = new Dictionary<int, Dictionary<Suite, int>>
            {
                { 1, new() { {Suite.Light, 1 }, { Suite.Nutrient, 5 }, { Suite.Water, 9 }, { Suite.Temperature, 8} } },
                { 2, new() { {Suite.Light, 5 }, { Suite.Nutrient, 6 }, { Suite.Water, 8 }, { Suite.Temperature, 8} } },
                { 3, new() { {Suite.Light, 9 }, { Suite.Nutrient, 7 }, { Suite.Water, 7 }, { Suite.Temperature, 8} } },
            };

            var plantData = new DevPlant(needs, seed.Number);
            var plant = new Plant(plantPosition, potData, soilData, plantData);
            AddEntity(plant);


            for (var i = 0; i < data.nSlots; i++)
            {
                var pos = Game.GetCardPosition(i) + potData.SlotOffset;
                if (i == 0)
                {
                    var seedCard = new Card(pos, seed);
                    var slot = new CardSlot(seedCard.Position, plantData.Tag, i);
                    AddEntity(seedCard);
                    AddEntity(slot);
                }
                else
                {
                    AddEntity(new CardSlot(pos, plantData.Tag, i));
                }

            }

        }

        private static int GetRandomStat() => GetRandomValue(Game.MinStat, Game.MaxStat);

        private static Suite GetRandomSuite() => Game.Suites[GetRandomValue(0, Game.Suites.Length - 1)];

        private static SuiteData GetSuiteRenderData((Suite suite, int stat) d) => d.suite switch
        {
            Suite.Seed => new(d.suite, d.stat, GetSuiteColors(d.suite)),
            Suite.Water => new(d.suite, d.stat, GetSuiteColors(d.suite)),
            Suite.Light => new(d.suite, d.stat, GetSuiteColors(d.suite)),
            Suite.Nutrient => new(d.suite, d.stat, GetSuiteColors(d.suite)),
            Suite.Temperature => new(d.suite, d.stat, GetSuiteColors(d.suite)),
            _ => throw new ArgumentOutOfRangeException(nameof(d.suite), d.suite, null)
        };

        public static (Color Render, Color Highlight) GetSuiteColors(Suite suite) => suite switch
        {
            Suite.Seed => (Color.Beige, Color.Brown),
            Suite.Water => (Color.SkyBlue, Color.Blue),
            Suite.Light => (Color.Yellow, Color.Gold),
            Suite.Nutrient => (Color.Green, Color.Lime),
            Suite.Temperature => (Color.Red, Color.Maroon),
            _ => throw new ArgumentOutOfRangeException(nameof(suite), suite, null)
        };

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
            return new(center, points, uv, new Texture2D { Id = 1 }, Color.Brown);
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
                RimColor: Color.DarkBrown,
                BasinLeftUp: offsetBasin,
                BasinLeftDown: offsetBasin + new Vector2(data.BasinSlant, basinHeight),
                BasinRightDown: offsetBasin + new Vector2(basinWidth + data.BasinSlant, basinHeight),
                BasinRightUp: offsetBasin + new Vector2(basinWidth + 2 * data.BasinSlant, 0),
                BasinThickness: data.BasinThickness,
                BasinColor: Color.DarkBrown,
                SlotOffset: new((rimWidth - basinWidth) / 2, data.RimThickness + (basinHeight - Card.Height - CardSlot.LineWidth * 2) / 2)
            );
        }
    }
}
