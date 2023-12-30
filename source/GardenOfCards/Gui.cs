namespace GardenOfCards
{
	internal static class Gui
	{
		private static readonly Vector2 ButtonSize = new(128, 64);
		private static readonly Color FillColor = Color.DARKBLUE;
		private static readonly Color FocusColor = Color.BLUE;

		public static GuiContainer CreateTurnGui ( ) => GuiContainerBuilder.CreateNew(true, 0, "TurnGui").AddChildren(
			new Label
			{
				Position = new(Game.WindowWidth * .85f, Game.WindowHeight * .15f),
				Size = ButtonSize,
				DoCenterText = true,
				FillColor = FillColor,
				HasOutlines = false,
				UpdateAction = l => l.Text = $"Turn {GroundKeeper.CurrentTurn.Number}",
			},
			new Button
			{
				Position = new(Game.WindowWidth * .85f, Game.WindowHeight * .65f),
				Size = ButtonSize,
				DoCenterText = true,
				OnMouseLeftClick = e => new DealHand { GuiEntity = e },
				FocusColor = FocusColor,
				BaseColor = FillColor,
				HasOutlines = false,
				UpdateAction = l => l.Text = $"Deal Hand x {GroundKeeper.CurrentTurn.HandsLeft}"
			},
			new Button
			{
				Position = new(Game.WindowWidth * .85f, Game.WindowHeight * .75f),
				Size = ButtonSize,
				Text = "End Turn",
				DoCenterText = true,
				OnMouseLeftClick = e => new EndTurn { GuiEntity = e },
				FocusColor = FocusColor,
				BaseColor = FillColor,
				HasOutlines = false
			},
			new Button
			{
				Position = new(Game.WindowWidth * .85f, Game.WindowHeight * .85f),
				Size = ButtonSize,
				Text = "Reset",
				DoCenterText = true,
				OnMouseLeftClick = e => new ResetGame { GuiEntity = e },
				FocusColor = FocusColor,
				BaseColor = FillColor,
				HasOutlines = false
			})
			.OnGuiEvent(( e, c ) =>
			{
				switch ( e )
				{
					case EndTurn:
					GroundKeeper.OnTurnEnd( );
					break;

					case ResetGame:
					RemoveEntitiesOfType<Card>( );
					RemoveEntitiesOfType<CardSlot>( );
					RemoveEntitiesOfType<Plant>( );
					GroundKeeper.OnGameStart(new GameStartData(new TurnData( ), new PotData( )));
					break;

					case DealHand when GroundKeeper.CurrentTurn.HandsLeft == 0:
					break;

					case DealHand:
					GroundKeeper.OnDealHand( );
					break;
				}
			});
	}
}