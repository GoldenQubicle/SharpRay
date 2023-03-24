namespace GardenOfCards
{
	public static class Game
	{
		internal static readonly Card BlankCard = new( );

		internal const int WindowWidth = 1080;
		internal const int WindowHeight = 980;

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

			GroundKeeper.OnGameStart(new GameStartData(new TurnData( ), new PotData( )));

			AddEntity(CreateTurnGui( ));

			

			Run( );
		}

		private static GuiContainer CreateTurnGui ( ) => GuiContainerBuilder.CreateNew(true, 0, "TurnGui").AddChildren(
			new Label
			{
				Position = new Vector2(WindowWidth * .85f, WindowHeight * .15f),
				Size = new Vector2(128, 64),
				DoCenterText = true,
				Text = $"Turn {GroundKeeper.CurrentTurn.Number}",
				FillColor = Color.DARKBLUE,
				HasOutlines = false
			},
			new Button
			{
				Position = new Vector2(WindowWidth * .85f, WindowHeight * .75f),
				Size = new Vector2(128, 64),
				Text = "End Turn",
				DoCenterText = true,
				OnMouseLeftClick = e => new EndTurn { GuiEntity = e },
				FocusColor = Color.BLUE,
				BaseColor = Color.DARKBLUE,
				HasOutlines = false
			},
			new Button
			{
				Position = new Vector2(WindowWidth * .85f, WindowHeight * .85f),
				Size = new Vector2(128, 64),
				Text = "Reset",
				DoCenterText = true,
				OnMouseLeftClick = e => new ResetGame { GuiEntity = e },
				FocusColor = Color.BLUE,
				BaseColor = Color.DARKBLUE,
				HasOutlines = false
			})
			.OnGuiEvent(( e, c ) =>
			{
				if ( e is EndTurn )
				{
					GroundKeeper.OnTurnEnd( );
					c.GetEntity<Label>( ).Text = $"Turn {GroundKeeper.CurrentTurn.Number}";
				}

				if ( e is ResetGame )
				{
					c.GetEntity<Label>( ).Text = "Turn 1";
					RemoveEntitiesOfType<Card>( );
					RemoveEntitiesOfType<CardSlot>( );
					RemoveEntitiesOfType<Plant>( );

					GroundKeeper.OnGameStart(new GameStartData(new TurnData( ), new PotData( )));
				}
			});


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

