namespace GardenOfCards
{
    internal static class Gui
    {

        public static GuiContainer CreateTurnGui() => GuiContainerBuilder.CreateNew(true, 0, "TurnGui").AddChildren(
            new Label
            {
                Position = new Vector2(Game.WindowWidth * .85f, Game.WindowHeight * .15f),
                Size = new Vector2(128, 64),
                DoCenterText = true,
                Text = $"Turn {GroundKeeper.CurrentTurn.Number}",
                FillColor = Color.DARKBLUE,
                HasOutlines = false
            },
            new Button
            {
                Position = new Vector2(Game.WindowWidth * .85f, Game.WindowHeight * .65f),
                Size = new Vector2(128, 64),
                Text = "Deal Hand",
                DoCenterText = true,
                OnMouseLeftClick = e => new DealHand { GuiEntity = e },
                FocusColor = Color.BLUE,
                BaseColor = Color.DARKBLUE,
                HasOutlines = false
            },
            new Button
            {
                Position = new Vector2(Game.WindowWidth * .85f, Game.WindowHeight * .75f),
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
                Position = new Vector2(Game.WindowWidth * .85f, Game.WindowHeight * .85f),
                Size = new Vector2(128, 64),
                Text = "Reset",
                DoCenterText = true,
                OnMouseLeftClick = e => new ResetGame { GuiEntity = e },
                FocusColor = Color.BLUE,
                BaseColor = Color.DARKBLUE,
                HasOutlines = false
            })
            .OnGuiEvent((e, c) =>
            {
                if (e is EndTurn)
                {
                    GroundKeeper.OnTurnEnd();
                    c.GetEntity<Label>().Text = $"Turn {GroundKeeper.CurrentTurn.Number}";
                }

                if (e is ResetGame)
                {
                    c.GetEntity<Label>().Text = "Turn 1";
                    RemoveEntitiesOfType<Card>();
                    RemoveEntitiesOfType<CardSlot>();
                    RemoveEntitiesOfType<Plant>();

                    GroundKeeper.OnGameStart(new GameStartData(new TurnData(), new PotData()));
                }

                if(e is DealHand)
                {
                    GroundKeeper.OnDealHand();
                }
            });
    }
}