namespace TerribleTetris
{
	internal class Gui
	{
		internal const string StartMenu = nameof(StartMenu);
		internal const string ScoreMenu = nameof(ScoreMenu);

		internal record StartGame(GuiEntity GuiEntity, Mode GameMode) : IGuiEvent;

		public static Label CreateMatchNotification() =>
			new Label
			{
				Position = new(WindowWidth - 300, WindowHeight - ( 1 + GetEntities<Label>( ).Count( ) ) * 35),
				Size = new(0, 25),
				Text = "Perfect Match!",
				TextColor = Color.RAYWHITE,
				HasOutlines = false,
				TextOffSet = new(3, 5),
				FillColor = Color.BLANK,
				TriggerTime = 500d * SharpRayConfig.TickMultiplier,
				UpdateAction = l =>
				{

					//fade out text
					if (l.ELapsedTime > l.TriggerTime)
					{
						var a = (float)MapRange(l.ELapsedTime, l.TriggerTime, 1000 * SharpRayConfig.TickMultiplier, 1, 0);
						l.TextColor = Fade(l.TextColor, a);
						if (a < 0)
							RemoveEntity(l);
					}
				}
			};


		public static GuiContainer CreateStartMenu()
		{
			var container = GuiContainerBuilder.CreateNew(tag: StartMenu);
			var btnWidth = 100;
			var btnHeight = 50;
			var btnPos = new Vector2(-128, -128);
			var btnMargin = 28;

			container.AddChildren(new Label
			{
				Position = new(0, 0),
				FillColor = BLUE,
				TextColor = SKYBLUE,
				Size = new(400, 480),
				Text = "Terrible Tetris",
				DoCenterText = false,
				TextOffSet = new(WindowWidth * .015f, WindowHeight * .05f),

				FontSize = 24
			},
				new Button
				{
					Position = btnPos,
					FocusColor = PURPLE,
					BaseColor = DARKPURPLE,
					TextColor = PINK,
					Size = new(btnWidth, btnHeight),
					Text = "Random",
					DoCenterText = true,
					OnMouseLeftClick = b => new StartGame(b, Mode.Generation)
				});

			var row = 1;

			foreach (var file in Directory.EnumerateFiles(AssestsFolder).Select((f, i) => (Path.GetFileNameWithoutExtension(f), i)))
			{
				var col = ( file.i + 1 ) % 3;
				var x = btnPos.X + ( ( btnWidth + btnMargin ) * col );
				var y = btnPos.Y + ( ( btnHeight + btnMargin ) * row );

				if (col % 3 == 0)
					row++;

				container.AddChildren(new Button
				{
					Position = new Vector2(x, y),
					Text = file.Item1,
					Tag = file.Item1,
					Size = new(100, 50),
					DoCenterText = true,
					OnMouseLeftClick = b => new StartGame(b, Mode.Playing)
				});
			}

			container.OnGuiEvent((e, c) =>
			{
				if (e is StartGame { GameMode: Mode.Generation } sg)
				{
					c.Hide( );
					StartGame(sg.GameMode);
				}

				if (e is StartGame { GameMode: Mode.Playing } sgp)
				{
					c.Hide( );
					StartGame(sgp.GameMode, $"{sgp.GuiEntity.Tag}.json");
				}
			});

			container.Translate(new(WindowWidth * .5f, WindowHeight * .5f));

			return container;
		}

	}
}
