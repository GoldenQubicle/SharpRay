namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate
    {
        private readonly PotRenderData _pot;
        private readonly SoilRenderData _soil;
        private readonly Dictionary<Suite, int> _stats = Enum.GetValues<Suite>().ToDictionary(s => s, _ => 0);

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
            DrawPot();
        }

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
