using GardenOfCards.Events;

namespace GardenOfCards
{
    public static class Game
    {
        internal const int WindowWidth = 1080;
        internal const int WindowHeight = 720;

        static async Task Main(string[] args)
        {
            Initialize(new()
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = Color.BEIGE,
                ShowFPS = true,
                DoEventLogging = false
            });

            
            AddEntity(CreateTurnGui());

            GroundKeeper.OnGameStart();

            Run();
        }

        private static GuiContainer CreateTurnGui() => GuiContainerBuilder.CreateNew(true, 0)
            .AddChildren(new Button
            {
                Position = new Vector2(WindowWidth * .85f, WindowHeight * .85f),
                Size = new Vector2(128, 64),
                Text = "End Turn",
                DoCenterText= true,
                OnMouseLeftClick = e => new EndTurn { GuiEntity = e }
            })
            .OnGuiEvent((e, c) =>
            {
               if(e is EndTurn)
                {
                    GroundKeeper.OnTurnEnd();
                }
            });



        /// <summary>
        /// Gets the card position for a given total of cards.
        /// Position is relative from 0,0 and accounts for card margin & card slot line width
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static Vector2 GetCardPosition(int idx, int total)
        {

            var spacing = Card.Margin + (CardSlot.LineWidth * 2);
            var relativeXPos = idx * Card.Width + idx * spacing;
            return new(relativeXPos + CardSlot.LineWidth, CardSlot.LineWidth);
        }

        public static float GetWidthForNCards(int total) => 
            (total * (Card.Width + 2 * CardSlot.LineWidth) + (total - 1) * Card.Margin);

    }
}

