namespace GardenOfCards.GameData
{
    internal abstract class PlantGrowthData
    {
        public abstract string Tag { get; }
        private Dictionary<int, Dictionary<Suite, int>> Needs { get; }
        public int SeedStat { get; }

        protected Dictionary<Suite, int> Stats = Enum.GetValues<Suite>().ToDictionary(s => s, _ => 0);
        
        protected PlantGrowthData(Dictionary<int, Dictionary<Suite, int>> needs, int seedStat)
        {
            Needs = needs;
            SeedStat = seedStat;
        }

        public abstract void OnTurnEnd(List<Card> cards);

        public void DebugDrawNeedsAndStats()
        {
            var turn = GroundKeeper.CurrentTurn.Number;

            if (turn > Needs.Count) turn = Needs.Count;

            var margin = new Vector2(0, 10);
            var graphHeight = 25 * Game.Suites.Length;

            const int turnGraphWidth = 90;
            const int overallGraphWidth = 100;
            var turnGraphAnchor = new Vector2(100, 100);
            var overallGraphAnchor = new Vector2(220, 100);

            var suiteSum = 0f;

            for (var idx = 0; idx < Game.Suites.Length; idx++)
            {
                var suite = Game.Suites[idx];
                var width = 10 * Needs[turn][suite];
                DrawRectangleV(turnGraphAnchor + new Vector2(0, 10 + idx * 25), new(width, 20), GroundKeeper.GetSuiteColors(suite).Render);

                var barOffset = GetBarOffset(idx);
                var sum = LerpStat(Stats[suite], Needs.Sum(t => t.Value[suite]), 0, 0);
                suiteSum += sum;
                DrawRectangleV(overallGraphAnchor + barOffset, new(100, 20), GroundKeeper.GetSuiteColors(suite).Render);
                DrawRectangleV(overallGraphAnchor + barOffset, new(sum, 20), GroundKeeper.GetSuiteColors(suite).Highlight);
                DrawTextV($"{sum}%", overallGraphAnchor + new Vector2(110, 10) + barOffset, 12, Color.Black);
            }
           
            
            DrawTextV("Needs per turn", turnGraphAnchor - margin, 12, Color.Black);
            DrawRectangleLinesV(turnGraphAnchor + new Vector2(0, 10), new(turnGraphWidth, graphHeight), Color.Black);

            DrawTextV($"Needs overall {(int)LerpStat((int)suiteSum, 300, 0, 0)} %", new(220, 90), 12, Color.Black);
            DrawRectangleLinesV(overallGraphAnchor + new Vector2(0, 10), new(overallGraphWidth, graphHeight), Color.Black);
        }

        private static Vector2 GetBarOffset(int idx) => new(0, 10 + idx * 25);

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
