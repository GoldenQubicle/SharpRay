namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate, IHasCollision
    {
        private readonly PotRenderData _potRenderData;

        public Plant(Vector2 position, PotRenderData potRenderData)
        {
            Position = position;
            _potRenderData = potRenderData;

            RenderLayer = 0;
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


        public void OnCollision(IHasCollider e)
        {
            if (e is Card c)
            {

            }
        }
    }
}
