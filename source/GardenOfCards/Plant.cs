namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate, IHasCollision
    {
        private int total = 1;
        private Texture2D texture;
        public Plant(Vector2 position)
        {
            Position = position;
            texture = GetTexture2D("PlantPot");

            for (var i = 0; i < total; i++)
            {
                var pos = Game.GetCardPosition(i, total) + new Vector2(28, 0); // + new Vector2(texture.width*.2f, texture.height*.35f);
                var cardSlot = new CardSlot(pos);
                AddEntity(cardSlot);
            }

            RenderLayer = 0;
        }

        public override void Render()
        {
            //DrawTextureV(texture, Position, Color.WHITE);

            var w = Card.Width * 1.63f;
            var h = Card.Height * 1.1f;
            var s = 22;
            DrawLineEx(Vector2.Zero, new (s, h), 5f, Color.BROWN);
            
            DrawLineEx(new(w, 0 ), new (w - s, h), 5f, Color.BROWN);
            DrawLineEx(new(s, h), new(w - s, h), 5f, Color.BROWN);
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
