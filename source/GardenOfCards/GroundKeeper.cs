using Rectangle = Raylib_cs.Rectangle;

namespace GardenOfCards
{
    internal class GroundKeeper : Entity, IHasRender 
    {
        public int HandSize { get; set; } = 4;

        private const int CardMargin = 15;

        private List<Card> _currentHand = new();
        private List<Plant> _plants = new();

        public void OnTurnStart()
        {
            _currentHand = Enumerable.Range(0, HandSize)
                .Select(n => new Card(GetCardPosition(n))
                {
                    ColorDefault = Color.WHITE,
                    ColorFocused = Color.RED,
                })
                .ToList();

            _currentHand.ForEach(AddEntity);
        }

        private Vector2 GetCardPosition(int idx)
        {
            var totalWidth = HandSize * Card.Width + HandSize * CardMargin + CardMargin;
            var relativeXPos = idx * Card.Width + idx * CardMargin + CardMargin;
            return new(relativeXPos + (Game.WindowWidth - totalWidth) /2, Game.WindowHeight / 2);
        }


        public void OnTurnEnd()
        {

        }

      

        //public override void Render()
        //{
        //    for (var i = 0; i < HandSize; i++)
        //    {
        //        var pos = GetCardPosition(i);
        //        var rect =  new Rectangle(pos.X, pos.Y, Card.Width, Card.Height);
        //    }
        //}
    }
}
