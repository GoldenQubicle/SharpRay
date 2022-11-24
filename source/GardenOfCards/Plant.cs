using Rectangle = Raylib_cs.Rectangle;

namespace GardenOfCards
{
    internal class Plant : Entity, IHasRender, IHasUpdate, IHasCollision
    {
        //private int total = 2;

        public Plant(Vector2 position)
        {
            Position = position;

            //for (var i = 0; i < total; i++)
            //{
            //    var pos = Game.GetCardPosition(i, total) + new Vector2(22, 0);
            //    var cardSlot = new CardSlot(pos);
            //    AddEntity(cardSlot);
            //}

            RenderLayer = 0;
        }

        public override void Render()
        {
            //DrawTextureV(texture, Position, Color.WHITE);
            var total = 3;
            var w = Game.GetWidthForNCards(total);
            var h = Card.Height * 1.75f;
            var s = 30;

            //rim
            var rimThickness = 50;
            var rimWidth = w + 4*s;
            DrawLineEx(new(0, rimThickness / 2), new(rimWidth, rimThickness / 2), rimThickness, Color.BROWN);

            var offsetBasin = new Vector2(s, rimThickness);

            //basin
            DrawLineEx(offsetBasin + Vector2.Zero, offsetBasin + new Vector2(s, h), 5f, Color.BROWN);
            DrawLineEx(offsetBasin + new Vector2(s, h), offsetBasin + new Vector2(w + s, h), 5f, Color.BROWN);
            DrawLineEx(offsetBasin + new Vector2(w + s, h), offsetBasin + new Vector2(w + 2 * s, 0), 5f, Color.BROWN);

            var offSetSlot = new Vector2((rimWidth - w) / 2 - s, rimThickness + (h-Card.Height)/2);

            //temp card slot rendering
            for (var i = 0; i < total; i++)
            {
                var pos = Game.GetCardPosition(i, total) + new Vector2(s, 0);
                var rect = new Rectangle()
                {
                    x = pos.X + offSetSlot.X,
                    y = pos.Y + offSetSlot.Y,
                    width = Card.Width,
                    height = Card.Height,
                };
                DrawRectangleRoundedLines(rect, Card.Roundness, 8, CardSlot.LineWidth, Color.DARKBROWN);
            }

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
