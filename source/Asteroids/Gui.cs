namespace Asteroids
{
    public static class Gui
    {
        public static class Tags
        {
            public const string ShipSelection = nameof(ShipSelection);
            public const string ScoreOverlay = nameof(ScoreOverlay);
            public const string Health = nameof(Health);
            public const string HealthBar = nameof(HealthBar);
            public const string Score = nameof(Score);
            public const string ScoreBar = nameof(ScoreBar);
            public const string StartGame = nameof(StartGame);
            public const string ShipSelectRight = nameof(ShipSelectRight);
            public const string ShipSelectLeft = nameof(ShipSelectLeft);
        }

        public const string SelectionSound = nameof(SelectionSound);
        public static string PlayerLifeIcon(int n) => $"PlayerLife{n}";
        public static string GetScoreString(int s) => $"Score : {s}";
        public static string GetHealthString(int h) => $"Health : {h}";

        //ship colors
        public const string Blue = "blue";
        public const string Red = "red";
        public const string Green = "green";
        public const string Orange = "orange";

        private static readonly Dictionary<string, Color> GuiShipBaseColor = new()
        {
            { Blue, Color.DARKBLUE },
            { Red, Color.MAROON },
            { Green, Color.LIME },
            { Orange, new Color(200, 100, 0, 255) },
        };

        private static readonly Dictionary<string, Color> GuiShipFocusColor = new()
        {
            { Blue, Color.BLUE },
            { Red, Color.RED },
            { Green, Color.GREEN },
            { Orange, Color.ORANGE },
        };

        public static Label CreatePickUpNotification(string description) => new Label
        {
            RenderLayer = RlGuiScoreOverlay,
            Position = new(WindowWidth - 300, WindowHeight - (1 + GetEntities<Label>().Count()) * 35),
            Size = new(0, 25),
            Text = description,
            TextColor = Color.RAYWHITE,
            WordWrap = false,
            HasOutlines = false,
            TextOffSet = new(3, 5),
            FillColor = BackGroundColor,
            TriggerTime = 2000d * SharpRayConfig.TickMultiplier,
            UpdateAction = l =>
            {
                if (l.Size.X < 200)
                {
                    l.Size += new Vector2(5f, 0f);
                    l.Position += new Vector2(2.5f, 0);
                }

                if (l.ELapsedTime > l.TriggerTime)
                {
                    var a = (float)MapRange(l.ELapsedTime, l.TriggerTime, 2750 * SharpRayConfig.TickMultiplier, 1, 0);
                    l.TextColor = Fade(l.TextColor, a);
                    if (a < 0) RemoveEntity(l);
                }
            }
        };

        public static GuiContainer CreateLevelWin(string desc) =>
            GuiContainerBuilder.CreateNew(isVisible: true).AddChildren(
                new Label
                {
                    Size = new Vector2(384, 164),
                    FillColor = Color.LIME,
                    Text = $"{desc} Cleared!",
                    TextColor = Color.GOLD,
                    FontSize = 32,
                    Font = GetFont(FontFuture),
                    TextOffSet = new Vector2(24, 32)
                },
                new Button
                {
                    Size = new Vector2(150, 35),
                    Position = new Vector2(0, 32),
                    BaseColor = Color.DARKGREEN,
                    FocusColor = Color.GREEN,
                    Text = "Next Level",
                    FontSize = 16,
                    Font = GetFont(FontFutureThin),
                    TextOffSet = new Vector2(24, 10),
                    OnMouseLeftClick = e => new NextLevel { GuiEntity = e }
                })
            .Translate(new Vector2(WindowWidth / 2, WindowHeight / 2))
            .OnGuiEvent((e, c) =>
            {
                if (e is NextLevel nl)
                {
                    c.EmitEvent(e);
                    RemoveEntity(c);
                    PlaySound(Game.StartSound);
                }
            });

        public static GuiContainer CreateShipLostNotification(int lifesLeft) =>
            GuiContainerBuilder.CreateNew(isVisible: true, renderLayer: RlGuiScoreOverlay).AddChildren(
                new Label
                {
                    Size = new Vector2(320, 128),
                    FillColor = Color.MAROON,
                    TextColor = Color.RAYWHITE,
                    FontSize = 24,
                    TextOffSet = new Vector2(0, 24),
                    Font = GetFont(FontFuture),
                    Text = lifesLeft == 0 
                        ? $"              GAME OVER" 
                        :  $"      You lost a ship! \n     {lifesLeft} ships remaining",
                },
                new Label
                {
                    Size = new Vector2(200, 30),
                    Position = new Vector2(0, 64),
                    FillColor = Color.BLANK,
                    TextColor = Color.RAYWHITE,
                    FontSize = 10,
                    Text = "Press space bar to continue",
                    HasOutlines = false,
                    TextOffSet = new Vector2(15, 0),
                    Font = GetFont(FontFutureThin),
                })
                .Translate(new Vector2(WindowWidth / 2, WindowHeight / 2))
                .OnKeyBoardEvent((e, c) =>
                {
                    if (e is KeySpaceBarPressed && IsPaused)
                    {
                        RemoveEntity(c);
                        
                        if (lifesLeft == 0)
                        {
                            ResetGame();
                            GetEntityByTag<GuiContainer>(Tags.ShipSelection).Show();
                            PlaySound(SelectionSound);
                            return;
                        }

                        IsPaused = false;

                    }
                });

        public static GuiContainer CreateScoreOverLay(int playerLifes, int lvlScore)
        {
            //create container 
            var container = GuiContainerBuilder.CreateNew(isVisible: true, tag: Tags.ScoreOverlay, renderLayer: RlGuiScoreOverlay);

            //add score & health displays
            container.AddChildren(
                new Label
                {
                    Tag = Tags.ScoreBar,
                    Position = new Vector2(WindowWidth - 200, 32),
                    Size = new Vector2(0, 50),
                    HasOutlines = false,
                    FillColor = Color.LIME,
                },
                new Label
                {
                    Tag = Tags.Score,
                    Position = new Vector2(WindowWidth - 200, 32),
                    Size = new Vector2(230, 50),
                    Text = GetScoreString(Score),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    TextOffSet = new Vector2(20, 10),
                    Font = GetFont(FontFutureThin),
                },
                new Label
                {
                    Tag = Tags.HealthBar,
                    Position = new Vector2(WindowWidth - 500, 32),
                    Size = new Vector2(0, 50),
                    HasOutlines = false,
                    FillColor = Color.RED,
                },
                new Label
                {
                    Tag = Tags.Health,
                    Position = new Vector2(WindowWidth - 500, 32),
                    Size = new Vector2(230, 50),
                    Text = GetHealthString(MaxHealth),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    TextOffSet = new Vector2(20, 10),
                    Font = GetFont(FontFutureThin),
                })
                .OnGameEvent((e, c) =>
                {
                    if (e is ShipHitAsteroid sha)
                    {
                        var health = sha.LifeLost ? MaxHealth : sha.ShipHealth;

                        UpdateHealthOverlay(c, health);

                        if (sha.LifeLost)
                        {
                            c.GetEntityByTag<ImageTexture>(PlayerLifeIcon(sha.LifeIconIdx)).Color = Color.DARKGRAY;
                        }
                    }

                    if (e is AsteroidDestroyed ad)
                    {
                        var sb = c.GetEntityByTag<Label>(Tags.ScoreBar);
                        var s = MapRange(Score, 0, lvlScore, 0, 230);
                        sb.Size = new(s, 50);
                        sb.Position = new(WindowWidth - 315 + s / 2, 32);

                        c.GetEntityByTag<Label>(Tags.Score).Text = GetScoreString(Score);
                    }
                });

            //add player life icons
            var icon = GetTexture2D(shipsIcons[ShipType][ShipColor]);

            for (var i = 1; i <= playerLifes; i++)
            {
                var pos = new Vector2(icon.width + (i * icon.width * 1.5f), 20);
                container.AddChildren(
                     new ImageTexture(icon, Color.WHITE)
                     {
                         Tag = PlayerLifeIcon(i),
                         Position = pos,
                     });
            }
            return container;
        }

        public static void UpdateHealthOverlay(GuiContainer c, int health)
        {
            var sb = c.GetEntityByTag<Label>(Tags.HealthBar);
            var s = MapRange(health, MaxHealth, 0, 0, 230);
            sb.Size = new(s, 50);
            sb.Position = new(WindowWidth - 615 + s / 2, 32);

            c.GetEntityByTag<Label>(Tags.Health).Text = GetHealthString(health);
        }

        public static GuiContainer CreateShipSelectionMenu() =>
           GuiContainerBuilder.CreateNew(isVisible: false, tag: Tags.ShipSelection, renderLayer: RlGuiShipSelection).AddChildren(
               new Label
               {
                   Text = "Meteor Madness",
                   TextColor = Color.YELLOW,
                   FillColor = GuiShipBaseColor[ShipColor],
                   FontSize = 45,
                   Position = new Vector2((WindowWidth / 2), WindowHeight / 8),
                   Size = new Vector2(600, 100),
                   TextOffSet = new Vector2(72, 30),
                   Font = GetFont(FontFuture),
               },
               new ImageTexture(GetTexture2D(ships[ShipType][ShipColor]), Color.WHITE)
               {
                   Position = new Vector2(WindowWidth / 2, WindowHeight / 2) -
                   new Vector2(GetTexture2D(ships[ShipType][ShipColor]).width / 2, GetTexture2D(ships[ShipType][ShipColor]).height / 2) // rather stupid tbh
               },
               new Button
               {
                   Tag = Tags.ShipSelectLeft,
                   Position = new Vector2((WindowWidth * .2f) - 15f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new ChangeShipType
                   {
                       GuiEntity = e,
                       ShipType = ShipType == 1 ? 3 : ShipType - 1
                   }
               },
               new Button
               {
                   Tag = Tags.ShipSelectRight,
                   Position = new Vector2((WindowWidth * .8f) + 15f, WindowHeight / 2),
                   Size = new Vector2(20, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new ChangeShipType
                   {
                       GuiEntity = e,
                       ShipType = ShipType == 3 ? 1 : ShipType + 1
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .2f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Blue],
                   FocusColor = GuiShipFocusColor[Blue],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Blue
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .4f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Green],
                   FocusColor = GuiShipFocusColor[Green],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Green
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .6f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Red],
                   FocusColor = GuiShipFocusColor[Red],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Red
                   }
               },
               new Button
               {
                   Position = new Vector2(WindowWidth * .8f, WindowHeight * .75f),
                   Size = new Vector2(50, 20),
                   BaseColor = GuiShipBaseColor[Orange],
                   FocusColor = GuiShipFocusColor[Orange],
                   OnMouseLeftClick = e => new ChangeShipColor
                   {
                       GuiEntity = e,
                       ShipColor = Orange
                   }
               },
               new Button
               {
                   Tag = Tags.StartGame,
                   Text = "Start",
                   TextColor = Color.YELLOW,
                   FontSize = 24,
                   Font = GetFont(FontFutureThin),
                   TextOffSet = new Vector2(28, 15),
                   Position = new Vector2(WindowWidth * .5f, WindowHeight * .9f),
                   Size = new Vector2(125, 50),
                   BaseColor = GuiShipBaseColor[ShipColor],
                   FocusColor = GuiShipFocusColor[ShipColor],
                   OnMouseLeftClick = e => new GameStart { GuiEntity = e }
               })
           .OnGuiEvent((e, c) =>
           {
               if (e is GameStart gs)
               {
                   c.Hide();
                   StopSound(Sounds[SelectionSound]);
                   PlaySound(Game.StartSound);
                   StartGame(0);
               }

               if (e is ChangeShipType cst)
               {
                   ShipType = cst.ShipType;
                   var texture = GetTexture2D(ships[ShipType][ShipColor]);
                   c.GetEntity<ImageTexture>().Texture2D = texture;
                   c.GetEntity<ImageTexture>().Position = new Vector2(WindowWidth / 2, WindowHeight / 2) - new Vector2(texture.width / 2, texture.height / 2);
               }

               if (e is ChangeShipColor csc)
               {
                   ShipColor = csc.ShipColor;
                   c.GetEntity<ImageTexture>().Texture2D = GetTexture2D(ships[ShipType][ShipColor]);
                   c.GetEntity<Label>().FillColor = GuiShipBaseColor[ShipColor];
                   c.GetEntities<Button>()
                        .Where(b => b.Tag.Equals(Tags.ShipSelectLeft) || b.Tag.Equals(Tags.ShipSelectRight) || b.Tag.Equals(Tags.StartGame)).ToList()
                        .ForEach(b =>
                        {
                            b.BaseColor = GuiShipBaseColor[ShipColor];
                            b.FocusColor = GuiShipFocusColor[ShipColor];
                        });
               }
           });
    }
}
