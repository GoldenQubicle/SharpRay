namespace GardenOfCards.GameData
{
    internal abstract class PlantGrowthData
    {
        public abstract string Tag { get; }
        private Dictionary<int, Dictionary<Suite, int>> Needs { get; }
        public int SeedStat { get; }

        protected Dictionary<Suite, int> Stats => new()
        {
            { Suite.Light ,0 },
            { Suite.Nutrient ,0 },
            { Suite.Water ,0 },
            { Suite.Seed ,0 },
        };

        protected PlantGrowthData(Dictionary<int, Dictionary<Suite, int>> needs, int seedStat)
        {
            Needs = needs;
            SeedStat = seedStat;
        }

        public abstract void OnTurnEnd(List<Card> cards);

        public void DebugDrawNeedsAndStats()
        {
            var turn = GroundKeeper.CurrentTurn.Number;

            if (turn > 3) turn = 3; //hacky obviously

            DrawTextV("Needs per turn", new(100, 90), 12, Color.BLACK);
            var lWidth = 10 * Needs[turn][Suite.Light];
            var nWidth = 10 * Needs[turn][Suite.Nutrient];
            var wWidth = 10 * Needs[turn][Suite.Water];

            DrawRectangleLinesV(new(100, 110), new(90, 100), Color.BLACK);
            DrawRectangleV(new(100, 110), new(lWidth, 20), GroundKeeper.GetSuiteColors(Suite.Light).Render);
            DrawRectangleV(new(100, 140), new(nWidth, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Render);
            DrawRectangleV(new(100, 170), new(wWidth, 20), GroundKeeper.GetSuiteColors(Suite.Water).Render);

            var lSumP = LerpStat(Stats[Suite.Light], Needs.Sum(t => t.Value[Suite.Light]), 0, 0);
            var nSumP = LerpStat(Stats[Suite.Nutrient], Needs.Sum(t => t.Value[Suite.Nutrient]), 0, 0);
            var wSumP = LerpStat(Stats[Suite.Water], Needs.Sum(t => t.Value[Suite.Water]), 0, 0);
            var overallP = (int)LerpStat((int)(lSumP + nSumP + wSumP), 300, 0, 0);

            DrawTextV($"Needs overall {overallP} %", new(220, 90), 12, Color.BLACK);

            DrawRectangleLinesV(new(220, 110), new(100, 100), Color.BLACK);
            DrawRectangleV(new(220, 110), new(100, 20), GroundKeeper.GetSuiteColors(Suite.Light).Render);
            DrawRectangleV(new(220, 140), new(100, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Render);
            DrawRectangleV(new(220, 170), new(100, 20), GroundKeeper.GetSuiteColors(Suite.Water).Render);

            DrawRectangleV(new(220, 110), new(lSumP, 20), GroundKeeper.GetSuiteColors(Suite.Light).Highlight);
            DrawRectangleV(new(220, 140), new(nSumP, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Highlight);
            DrawRectangleV(new(220, 170), new(wSumP, 20), GroundKeeper.GetSuiteColors(Suite.Water).Highlight);

            DrawTextV($"{lSumP}%", new(330, 110), 12, Color.BLACK);
            DrawTextV($"{nSumP}%", new(330, 140), 12, Color.BLACK);
            DrawTextV($"{wSumP}%", new(330, 170), 12, Color.BLACK);
        }

        private static float LerpStat(int stat, int max = 9, int min = 1, int minS = 1) => MapRange(stat, min, max, minS, 100);

    }


    internal class DevPlant : PlantGrowthData
    {
        public override string Tag => nameof(DevPlant);

        public DevPlant(Dictionary<int, Dictionary<Suite, int>> needs, int seedStat) : base(needs, seedStat)
        {

        }



        public override void OnTurnEnd(List<Card> cards)
        {
            foreach (var card in cards)
                Stats[card.Suite] += card.Stat;
        }
    }
}
