namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate
    {
        private readonly PotRenderData _potRenderData;

        public Plant(Vector2 position, PotRenderData potRenderData)
        {
            Position = position;
            _potRenderData = potRenderData;
            RenderLayer = 0;
            Tag = "DevPlant"; // TODO pass in via seed card
        }

        public override void Render()
        {
            DrawPot();
        }

        private void DrawPot()
        {
            DrawLineEx(_potRenderData.RimStart, _potRenderData.RimEnd, _potRenderData.RimThickness, _potRenderData.RimColor);
            DrawLineEx(_potRenderData.BasinLeftUp, _potRenderData.BasinLeftDown, _potRenderData.BasinThickness, _potRenderData.BasinColor);
            DrawLineEx(_potRenderData.BasinLeftDown, _potRenderData.BasinRightDown, _potRenderData.BasinThickness, _potRenderData.BasinColor);
            DrawLineEx(_potRenderData.BasinRightDown, _potRenderData.BasinRightUp, _potRenderData.BasinThickness, _potRenderData.BasinColor);
        }

        public override void Update(double deltaTime)
        {

        }
    }
}
