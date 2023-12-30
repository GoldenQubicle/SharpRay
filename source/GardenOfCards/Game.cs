namespace GardenOfCards
{
	public static class Game
	{
		internal static readonly Card BlankCard = new( );

		internal const int WindowWidth = 1080;
		internal const int WindowHeight = 980;

        internal static readonly Suite[] Suites = Enum.GetValues<Suite>().Except(new[] { Suite.Seed }).ToArray();
        internal const int MinStat = 1;
        internal const int MaxStat = 9;

        static async Task Main ( string[ ] args )
		{
			Initialize(new( )
			{
				WindowWidth = WindowWidth,
				WindowHeight = WindowHeight,
				BackGroundColor = Color.GRAY,
				ShowFPS = true,
				DoEventLogging = false
			});

			GroundKeeper.OnGameStart(new GameStartData(new TurnData(), new PotData()));

			AddEntity(Gui.CreateTurnGui());

			Run( );
		}




        /// <summary>
        /// Gets the card position for a given total of cards.
        /// Position is relative from 0,0 and accounts for card margin & card slot line width
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static Vector2 GetCardPosition ( int idx )
		{
			var spacing = Card.Margin + ( CardSlot.LineWidth * 2 );
			var relativeXPos = idx * Card.Width + idx * spacing;
			return new(relativeXPos + CardSlot.LineWidth, CardSlot.LineWidth);
		}

		public static float GetWidthForNCards ( int total ) =>
			( total * ( Card.Width + 2 * CardSlot.LineWidth ) + ( total - 1 ) * Card.Margin );
    }
}

