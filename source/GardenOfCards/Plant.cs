namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate
    {
        private readonly PotRenderData _pot;
        private readonly SoilRenderData _soil;

        private readonly Dictionary<int, Dictionary<Suite, int>> _needs = new()
        {
            { 1, new() { {Suite.Light, 1 }, { Suite.Nutrient, 5 }, { Suite.Water, 9 } } },
            { 2, new() { {Suite.Light, 5 }, { Suite.Nutrient, 5 }, { Suite.Water, 8 } } },
            { 3, new() { {Suite.Light, 9 }, { Suite.Nutrient, 6 }, { Suite.Water, 9 } } },
        };

        private readonly Dictionary<Suite, int> _stats = new()
        {
            { Suite.Light ,0},
            { Suite.Nutrient ,0},
            { Suite.Water ,0},
            { Suite.Seed ,0},
        };

        public Plant(Vector2 position, PotRenderData potRenderData, SoilRenderData soilRenderData, string tag)
        {
            Position = position;
            _pot = potRenderData;
            _soil = soilRenderData;
            RenderLayer = 0;
            Tag = tag;
        }

        public override void Render()
        {
            DebugDrawNeedsAndStats();

            var start = _pot.RimEnd with { X = _pot.RimEnd.X - _pot.Width / 2 };
            var end = start with { Y = start.Y - 100 };
            var color = LerpColor(Color.LIME, Color.GOLD, .25f);
            DrawLineEx(start, end, 10, color);

            DrawPot();

        }

        private void DebugDrawNeedsAndStats()
        {
            var turn = GroundKeeper.CurrentTurn.Number;

            if (turn > 3) turn = 3; //hacky obviously

            DrawTextV("Needs per turn", new(100, 90), 12, Color.BLACK);
            var lWidth = LerpStat(_needs[turn][Suite.Light]);
            var nWidth = LerpStat(_needs[turn][Suite.Nutrient]);
            var wWidth = LerpStat(_needs[turn][Suite.Water]);

            DrawRectangleLinesV(new(100, 110), new(100, 100), Color.BLACK);
            DrawRectangleV(new(100, 110), new(lWidth, 20), GroundKeeper.GetSuiteColors(Suite.Light).Render);
            DrawRectangleV(new(100, 140), new(nWidth, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Render);
            DrawRectangleV(new(100, 170), new(wWidth, 20), GroundKeeper.GetSuiteColors(Suite.Water).Render);


            DrawTextV("Needs overall", new(220, 90), 12, Color.BLACK);
            var lSum = LerpStat(_needs.Sum(t => t.Value[Suite.Light]), 9 * 3);
            var nSum = LerpStat(_needs.Sum(t => t.Value[Suite.Nutrient]), 9 * 3);
            var wSum = LerpStat(_needs.Sum(t => t.Value[Suite.Water]), 9 * 3);


            var actualLight = LerpStat(_stats[Suite.Light], 9 * 3);
            var actualNutrient = LerpStat(_stats[Suite.Nutrient], 9 * 3);
            var actualWater = LerpStat(_stats[Suite.Water], 9 * 3);

            DrawRectangleLinesV(new(220, 110), new(100, 100), Color.BLACK);
            DrawRectangleV(new(220, 110), new(lSum, 20), GroundKeeper.GetSuiteColors(Suite.Light).Render);
            DrawRectangleV(new(220, 140), new(nSum, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Render);
            DrawRectangleV(new(220, 170), new(wSum, 20), GroundKeeper.GetSuiteColors(Suite.Water).Render);

            DrawRectangleV(new(220, 110), new(actualLight, 20), GroundKeeper.GetSuiteColors(Suite.Light).Highlight);
            DrawRectangleV(new(220, 140), new(actualNutrient, 20), GroundKeeper.GetSuiteColors(Suite.Nutrient).Highlight);
            DrawRectangleV(new(220, 170), new(actualWater, 20), GroundKeeper.GetSuiteColors(Suite.Water).Highlight);
        }

        private float LerpStat(int stat, int max = 9) => MapRange(stat, 1, max, 10, 100);

        private void DrawPot()
        {
            DrawTexturePoly(_soil.Texture, _soil.Center, _soil.Points, _soil.UV, _soil.Points.Length, _soil.Color);

            DrawLineEx(_pot.RimStart, _pot.RimEnd, _pot.RimThickness, _pot.RimColor);
            DrawLineEx(_pot.BasinLeftUp, _pot.BasinLeftDown, _pot.BasinThickness, _pot.BasinColor);
            DrawLineEx(_pot.BasinLeftDown, _pot.BasinRightDown, _pot.BasinThickness, _pot.BasinColor);
            DrawLineEx(_pot.BasinRightDown, _pot.BasinRightUp, _pot.BasinThickness, _pot.BasinColor);
        }

        public override void Update(double deltaTime)
        {

        }

        internal void OnTurnEnd(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
                _stats[card.Suite] += card.Stat;

        }
    }
}
