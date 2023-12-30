namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate
    {
        private readonly PotRenderData _pot;
        private readonly SoilRenderData _soil;
        private readonly PlantGrowthData _growthData;
        private readonly StemSegment _stemSegment;

        public Plant(Vector2 position, PotRenderData potRenderData, SoilRenderData soilRenderData, PlantGrowthData growthData)
        {
            Position = position;
            _pot = potRenderData;
            _soil = soilRenderData;
            _growthData = growthData;
            RenderLayer = 0;
            Tag = growthData.Tag;
            _stemSegment = new(_pot.RimEnd with { X = _pot.RimEnd.X - _pot.Width / 2 }, _growthData.SeedStat);

        }

        public override void Render()
        {
            _growthData.DebugDrawNeedsAndStats();

            _stemSegment.Render();

            DrawPot();
        }

  
        private void DrawPot()
        {
	        //NOTE: draw texture poly is no longer available in raylib 5, for some fucking reason
			//DrawTexturePoly(_soil.Texture, _soil.Center, _soil.Points, _soil.UV, _soil.Points.Length, _soil.Color);

			DrawLineEx(_pot.RimStart, _pot.RimEnd, _pot.RimThickness, _pot.RimColor);
            DrawLineEx(_pot.BasinLeftUp, _pot.BasinLeftDown, _pot.BasinThickness, _pot.BasinColor);
            DrawLineEx(_pot.BasinLeftDown, _pot.BasinRightDown, _pot.BasinThickness, _pot.BasinColor);
            DrawLineEx(_pot.BasinRightDown, _pot.BasinRightUp, _pot.BasinThickness, _pot.BasinColor);
        }

        public override void Update(double deltaTime)
        {

        }

        internal void OnTurnEnd(List<Card> cards)
        {
            _growthData.OnTurnEnd(cards);
            _stemSegment.OnTurnEnd();
        }
    }
}
