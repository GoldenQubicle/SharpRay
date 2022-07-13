namespace Asteroids
{
    public static class Gui
    {
        public static class Tags
        {
            public const string MainMenu = nameof(MainMenu);
            public const string ShipSelection = nameof(ShipSelection);
            public const string ScoreOverlay = nameof(ScoreOverlay);
            public const string Health = nameof(Health);
            public const string HealthBar = nameof(HealthBar);
            public const string Score = nameof(Score);
            public const string ScoreBar = nameof(ScoreBar);
            public const string StartGame = nameof(StartGame);
            public const string ShipSelectRight = nameof(ShipSelectRight);
            public const string ShipSelectLeft = nameof(ShipSelectLeft);
            public const string ShipTypeLabel = nameof(ShipTypeLabel);
        }

        public const string SelectionSound = nameof(SelectionSound);
        public const string ButtonClickSound = nameof(ButtonClickSound);
        public static string PlayerLifeIcon(int n) => $"PlayerLife{n}";
        public static string GetScoreString(int s) => $"Score : {s}";
        public static string GetHealthString(int h) => $"Health : {h}";

        public enum ShipColor
        {
            blue,
            red,
            green,
            orange
        }

        private static readonly Dictionary<ShipColor, Color> GuiShipBaseColor = new()
        {
            { ShipColor.blue, Color.DARKBLUE },
            { ShipColor.red, new Color(175, 25, 40, 255) },
            { ShipColor.green, Color.LIME },
            { ShipColor.orange, new Color(200, 100, 0, 255) },
        };

        private static readonly Dictionary<ShipColor, Color> GuiShipFocusColor = new()
        {
            { ShipColor.blue, Color.BLUE },
            { ShipColor.red, Color.RED },
            { ShipColor.green, Color.GREEN },
            { ShipColor.orange, Color.ORANGE },
        };

        private static readonly Dictionary<string, string> Credits = new()
        {
            { "Space Shooter Redux by Kenney.nl", "https://kenney.nl/assets/space-shooter-redux"},
            { "SciFi SoundFx by Kenney.nl", "https://kenney.nl/assets/sci-fi-sounds" },
            { "Additional SoundFx by MixKit","https://mixkit.co/free-sound-effects/video-game/" },
            { "Made with RayLib",  "https://www.raylib.com" }
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
            FillColor = Color.BLANK,
            TriggerTime = 2000d * SharpRayConfig.TickMultiplier,
            UpdateAction = l =>
            {
                //scroll text inwards, i.e. from fixed x position
                if (l.Size.X < 200)
                {
                    l.Size += new Vector2(5f, 0f);
                    l.Position += new Vector2(2.5f, 0);
                }

                //fade out text
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
                    FillColor = Color.DARKPURPLE,
                    Text = desc.Contains(Levels.Data.Count.ToString()) ? $"All Levels clear" :  $"{desc} Cleared",
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
                    Text = desc.Contains(Levels.Data.Count.ToString()) ? "To Main Menu" :"Next Level",
                    FontSize = 16,
                    Font = GetFont(FontFutureThin),
                    DoCenterText = true,
                    OnMouseLeftClick = e => new NextLevel { GuiEntity = e }
                })
            .Translate(new Vector2(WindowWidth / 2, WindowHeight / 2))
            .OnGuiEvent((e, c) =>
            {
                if (e is NextLevel nl)
                {
                    PlaySound(StartSound);
                    RemoveEntity(c);
                    HideCursor();
                    c.EmitEvent(e);
                }
            });

        public static GuiContainer CreateShipLostNotification() =>
            GuiContainerBuilder.CreateNew(isVisible: true, renderLayer: RlGuiScoreOverlay).AddChildren(
                new Label
                {
                    Size = new Vector2(320, 128),
                    FillColor = Color.MAROON,
                    TextColor = Color.RAYWHITE,
                    FontSize = 24,
                    TextOffSet = new Vector2(0, 24),
                    Font = GetFont(FontFuture),
                    Text = CurrentLifes == 0
                        ? $"              GAME OVER"
                        : $"      You lost a ship! \n     {CurrentLifes} ships remaining",
                },
                new Button
                {
                    Position = new Vector2(0, 42),
                    Size = new Vector2(100, 20),
                    Text = "Continue",
                    TextOffSet = new Vector2(11, 2),
                    TextColor = Color.RAYWHITE,
                    Font = GetFont(FontFutureThin),
                    FontSize = 16,
                    FocusColor = Color.LIGHTGRAY,
                    OnMouseLeftClick = b => new ContinueWithLevel { GuiEntity = b }
                })
                .Translate(new Vector2(WindowWidth / 2, WindowHeight / 2))
                .OnGuiEvent((e, c) => //TODO may want to have an onEnter key press which handles the notification as well?
                {
                    if (e is ContinueWithLevel && IsPaused)
                    {
                        PlaySound(ButtonClickSound);
                        RemoveEntity(c);

                        if (CurrentLifes == 0)
                        {
                            ResetGame();
                            RemoveEntity(GetEntityByTag<GuiContainer>(Tags.ScoreOverlay));
                            AddEntity(CreateMainMenu(true));
                            PlaySound(SelectionSound, isRepeated: true);
                            ShowCursor();
                            return;
                        }



                        HideCursor();
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
                    Text = GetScoreString(CurrentScore),
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
                        UpdateHealthOverlay(c, CurrentHealth);
                    }

                    if (e is ShipLifeLost sll)
                    {
                        c.GetEntityByTag<ImageTexture>(PlayerLifeIcon(sll.LifeIconIdx)).Color = Color.DARKGRAY;

                    }

                    if (e is AsteroidDestroyed ad)
                    {
                        var sb = c.GetEntityByTag<Label>(Tags.ScoreBar);
                        var s = MapRange(CurrentScore, 0, lvlScore, 0, 230);
                        sb.Size = new(s, 50);
                        sb.Position = new(WindowWidth - 315 + s / 2, 32);

                        c.GetEntityByTag<Label>(Tags.Score).Text = GetScoreString(CurrentScore);
                    }
                });

            //add player life icons
            var icon = GetTexture2D(shipsIcons[SelectedShipType][SelectedShipColor.ToString().ToLower()]);

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
            var s = MapRange(health < 0 ? 0 : health, MaxHealth, 0, 0, 230);
            sb.Size = new(s, 50);
            sb.Position = new(WindowWidth - 615 + s / 2, 32);

            c.GetEntityByTag<Label>(Tags.Health).Text = GetHealthString(health);
        }

        public static GuiContainer CreateMainMenu(bool isVisible = false) =>
           GuiContainerBuilder.CreateNew(tag: Tags.MainMenu, isVisible: isVisible, renderLayer: RlGuiShipSelection).AddChildren(
           GetTitleBanner(),
               new Button
               {
                   Tag = Tags.StartGame,
                   Text = "Start Game",
                   TextColor = Color.YELLOW,
                   FontSize = 32,
                   Font = GetFont(FontFutureThin),
                   DoCenterText = true,
                   Position = new Vector2(WindowWidth * 0.361f, WindowHeight * .3f),
                   Size = new Vector2(300, 50),
                   BaseColor = Color.DARKGREEN,
                   FocusColor = Color.LIME,
                   OnMouseLeftClick = e => new GameStart { GuiEntity = e },
               },
               new Button
               {
                   Tag = Tags.StartGame,
                   Text = "Select Ship",
                   TextColor = Color.YELLOW,
                   FontSize = 32,
                   Font = GetFont(FontFutureThin),
                   DoCenterText = true,
                   Position = new Vector2(WindowWidth * 0.361f, WindowHeight * .45f),
                   Size = new Vector2(300, 50),
                   BaseColor = Color.DARKGREEN,
                   FocusColor = Color.LIME,
                   OnMouseLeftClick = e => new SelectShip { GuiEntity = e }
               },
               new Button
               {
                   Text = "Credits",
                   TextColor = Color.YELLOW,
                   FontSize = 32,
                   Font = GetFont(FontFutureThin),
                   DoCenterText = true,
                   Position = new Vector2(WindowWidth * 0.361f, WindowHeight * .6f),
                   Size = new Vector2(300, 50),
                   BaseColor = Color.DARKGREEN,
                   FocusColor = Color.LIME,
                   OnMouseLeftClick = e => new ShowCredits { GuiEntity = e }
               },
             GuiContainerBuilder.CreateNew().AddChildren(
                  new ImageTexture(GetTexture2D(ships[SelectedShipType][SelectedShipColor]), Color.WHITE)
                  {
                      Position = new Vector2(-(GetTexture2D(ships[SelectedShipType][SelectedShipColor]).width / 2 - WindowWidth * .318f - 25), -216),
                  },
                  new ImageTexture(GetTexture2D(nameof(KeyLeftDown)), Color.WHITE)
                  {
                      Position = new Vector2(WindowWidth * .318f - 50, 25),
                      HasOutline = true,
                  },
                  new ImageTexture(GetTexture2D(nameof(KeyRightDown)), Color.WHITE)
                  {
                      Position = new Vector2(WindowWidth * .318f + 50, 25),
                      HasOutline = true,
                  },
                  new ImageTexture(GetTexture2D(nameof(KeyUpDown)), Color.WHITE)
                  {
                      Position = new Vector2(WindowWidth * .318f, -25),
                      HasOutline = true,
                  },
                  new Label
                  {
                      Position = new Vector2(WindowWidth * .318f + 25, 135),
                      Size = new Vector2(150, 50),
                      Text = "spacebar",
                      Font = GetFont(FontFutureThin),
                      FontSize = 24,
                      DoCenterText = true,
                      FillColor = Color.BLANK,
                      TextColor = Color.WHITE,
                      HasOutlines = true,
                  },
                  new Label
                  {
                      Position = new Vector2(0, 135),
                      Size = new Vector2(300, 50),
                      Text = "fire weapon",
                      FillColor = Color.BLANK,
                      HasOutlines = true,
                      Font = GetFont(FontFuture),
                      FontSize = 30,
                      DoCenterText = true,
                  },
                  new Label
                  {
                      Position = new Vector2(0, 50),
                      Size = new Vector2(300, 50),
                      Text = "ship control",
                      FillColor = Color.BLANK,
                      HasOutlines = true,
                      Font = GetFont(FontFuture),
                      FontSize = 30,
                      DoCenterText = true,
                  }).Translate(new(WindowWidth * .361f, WindowHeight * .7f)))
            .OnGuiEvent((e, c) =>
            {
                if (e is SelectShip ss)
                {
                    PlaySound(ButtonClickSound);
                    c.Hide();
                    AddEntity(CreateShipSelectionMenu(true));
                }

                if (e is GameStart gs)
                {
                    RemoveEntity(c);
                    StopSound(SelectionSound);
                    PlaySound(StartSound);
                    StartGame(0);
                }

                if (e is ShowCredits sc)
                {
                    PlaySound(ButtonClickSound);
                    c.Hide();
                    AddEntity(CreateCredits());
                }
            });

        private static Label GetTitleBanner() => new()
        {
            Text = "Meteor Mayhem",
            TextColor = Color.YELLOW,
            FillColor = Color.DARKPURPLE,
            FontSize = 72,
            Position = new Vector2(WindowWidth / 2, WindowHeight * .1f),
            Size = new Vector2(780, 100),
            Font = GetFont(FontFuture),
            DoCenterText = true
        };

        public static GuiContainer CreateCredits()
        {
            var c = GuiContainerBuilder.CreateNew().AddChildren(
                GetTitleBanner(),
                  new Label
                  {
                      Position = new Vector2(WindowWidth * .5f, WindowHeight * .50f),
                      Size = new Vector2(600, 400),
                      FillColor = Color.DARKGREEN,
                      TextColor = Color.YELLOW
                  },
                   new Button
                   {
                       Position = new Vector2(WindowWidth * .5f, WindowHeight * .9f),
                       Size = new Vector2(250, 50),
                       Text = "Back to Menu",
                       Font = GetFont(FontFutureThin),
                       FontSize = 28,
                       OnMouseLeftClick = e => new NextLevel { GuiEntity = e },
                       DoCenterText = true
                   });

            var links = GuiContainerBuilder.CreateNew();
            foreach (var t in Credits.Select((kvp, i) => (text: kvp.Key, uri: kvp.Value, idx: i)))
            {
                links.AddChildren(new Button
                {
                    Position = new Vector2(0f, t.idx * 85),
                    Size = new Vector2(500, 60),
                    Text = t.text,
                    OnMouseLeftClick = e => new OpenLink
                    {
                        GuiEntity = e,
                        URL = t.uri
                    },
                    Font = GetFont(FontFuture),
                    FontSize = 22,
                    TextOffSet = new Vector2(16, 18),
                    BaseColor = Color.DARKBLUE,
                    TextColor = Color.RAYWHITE,
                    FocusColor = Color.SKYBLUE,
                    DoCenterText = true,
                });
            }
            links.Translate(new Vector2(WindowWidth * .5f, WindowHeight * .32f))
                .OnGuiEvent((e, c) =>
                {
                    if (e is OpenLink ol)
                    {
                        OpenURL(ol.URL);
                    }
                });

            c.AddChildren(links)
                .Translate(new Vector2(0, 0))
                .OnGuiEvent((e, c) =>
                {
                    if (e is NextLevel nl)
                    {
                        PlaySound(ButtonClickSound);
                        RemoveEntity(c);
                        GetEntityByTag<GuiContainer>(Tags.MainMenu).Show();
                    }
                });

            return c;
        }

        public static GuiContainer CreateShipSelectionMenu(bool isVisible = false) =>
            GuiContainerBuilder.CreateNew(isVisible: isVisible, tag: Tags.ShipSelection, renderLayer: RlGuiShipSelection).AddChildren(
                new Label
                {
                    Text = "Meteor Mayhem",
                    TextColor = Color.YELLOW,
                    FillColor = GuiShipBaseColor[SelectedShipColor],
                    FontSize = 72,
                    Position = new Vector2(WindowWidth / 2, WindowHeight * .1f),
                    Size = new Vector2(780, 100),
                    Font = GetFont(FontFuture),
                    DoCenterText = true
                },
                new ImageTexture(GetTexture2D(ships[SelectedShipType][SelectedShipColor]), Color.WHITE)
                {
                    Position = new Vector2(WindowWidth / 2, WindowHeight / 2) -
                    new Vector2(GetTexture2D(ships[SelectedShipType][SelectedShipColor]).width / 2,
                            GetTexture2D(ships[SelectedShipType][SelectedShipColor]).height / 2) // rather stupid tbh
                },
                new Label
                {
                    Tag = Tags.ShipTypeLabel,
                    Position = new Vector2(WindowWidth * .5f, WindowHeight * .30f),
                    Size = new Vector2(200, 50),
                    Text = $"Type {SelectedShipType}",
                    DoCenterText = true,
                    FillColor = Color.BLANK,
                    HasOutlines = false,
                    Font = GetFont(FontFutureThin),
                    FontSize = 39,
                    TextColor = GuiShipBaseColor[SelectedShipColor]
                },
                new Button
                {
                    Tag = Tags.ShipSelectLeft,
                    Position = new Vector2((WindowWidth * .2f), WindowHeight * .49f),
                    Texture2D = GetTexture2D(nameof(KeyLeftDown)),
                    Size = new Vector2(GetTexture2D(nameof(KeyLeftDown)).width, GetTexture2D(nameof(KeyLeftDown)).height), // also kinda stupid tbh
                    BaseColor = GuiShipBaseColor[SelectedShipColor],
                    FocusColor = GuiShipFocusColor[SelectedShipColor],
                    OnMouseLeftClick = e => new ChangeShipType
                    {
                        GuiEntity = e,
                        ShipType = SelectedShipType == 1 ? 3 : SelectedShipType - 1
                    }
                },
                new Button
                {
                    Tag = Tags.ShipSelectRight,
                    Position = new Vector2((WindowWidth * .8f), WindowHeight * .49f),
                    Texture2D = GetTexture2D(nameof(KeyRightDown)),
                    Size = new Vector2(GetTexture2D(nameof(KeyRightDown)).width, GetTexture2D(nameof(KeyRightDown)).height), // also kinda stupid tbh
                    BaseColor = GuiShipBaseColor[SelectedShipColor],
                    FocusColor = GuiShipFocusColor[SelectedShipColor],
                    OnMouseLeftClick = e => new ChangeShipType
                    {
                        GuiEntity = e,
                        ShipType = SelectedShipType == 3 ? 1 : SelectedShipType + 1
                    }
                },
                new Button
                {
                    Position = new Vector2(WindowWidth * .2f, WindowHeight * .75f),
                    Size = new Vector2(50, 25),
                    BaseColor = GuiShipBaseColor[ShipColor.blue],
                    FocusColor = GuiShipFocusColor[ShipColor.blue],
                    HasOutlines = false,
                    OnMouseLeftClick = e => new ChangeShipColor
                    {
                        GuiEntity = e,
                        ShipColor = ShipColor.blue
                    }
                },
                new Button
                {
                    Position = new Vector2(WindowWidth * .4f, WindowHeight * .75f),
                    Size = new Vector2(50, 25),
                    BaseColor = GuiShipBaseColor[ShipColor.green],
                    FocusColor = GuiShipFocusColor[ShipColor.green],
                    HasOutlines = false,
                    OnMouseLeftClick = e => new ChangeShipColor
                    {
                        GuiEntity = e,
                        ShipColor = ShipColor.green
                    }
                },
                new Button
                {
                    Position = new Vector2(WindowWidth * .6f, WindowHeight * .75f),
                    Size = new Vector2(50, 25),
                    BaseColor = GuiShipBaseColor[ShipColor.red],
                    FocusColor = GuiShipFocusColor[ShipColor.red],
                    HasOutlines = false,
                    OnMouseLeftClick = e => new ChangeShipColor
                    {
                        GuiEntity = e,
                        ShipColor = ShipColor.red
                    }
                },
                new Button
                {
                    Position = new Vector2(WindowWidth * .8f, WindowHeight * .75f),
                    Size = new Vector2(50, 25),
                    BaseColor = GuiShipBaseColor[ShipColor.orange],
                    FocusColor = GuiShipFocusColor[ShipColor.orange],
                    HasOutlines = false,
                    OnMouseLeftClick = e => new ChangeShipColor
                    {
                        GuiEntity = e,
                        ShipColor = ShipColor.orange
                    }
                },
                new Button
                {
                    Tag = Tags.StartGame,
                    Text = "Start",
                    TextColor = Color.YELLOW,
                    FontSize = 24,
                    Font = GetFont(FontFutureThin),
                    DoCenterText = true,
                    Position = new Vector2(WindowWidth * .5f, WindowHeight * .9f),
                    Size = new Vector2(125, 50),
                    BaseColor = GuiShipBaseColor[SelectedShipColor],
                    FocusColor = GuiShipFocusColor[SelectedShipColor],
                    OnMouseLeftClick = e => new GameStart { GuiEntity = e }
                })
            .OnGuiEvent((e, c) =>
            {
                if (e is GameStart gs)
                {
                    RemoveEntity(c);
                    StopSound(SelectionSound);
                    PlaySound(StartSound);
                    StartGame(0);
                }

                if (e is ChangeShipType cst)
                {
                    PlaySound(ButtonClickSound);
                    SelectedShipType = cst.ShipType;
                    var texture = GetTexture2D(ships[SelectedShipType][SelectedShipColor]);
                    c.GetEntity<ImageTexture>().Texture2D = texture;
                    c.GetEntity<ImageTexture>().Position = new Vector2(WindowWidth / 2, WindowHeight / 2) - new Vector2(texture.width / 2, texture.height / 2);
                    c.GetEntityByTag<Label>(Tags.ShipTypeLabel).Text = $"Type {SelectedShipType}";
                }

                if (e is ChangeShipColor csc)
                {
                    PlaySound(ButtonClickSound);
                    SelectedShipColor = csc.ShipColor;
                    c.GetEntity<ImageTexture>().Texture2D = GetTexture2D(ships[SelectedShipType][SelectedShipColor]);
                    c.GetEntity<Label>().FillColor = GuiShipBaseColor[SelectedShipColor];
                    c.GetEntityByTag<Label>(Tags.ShipTypeLabel).TextColor = GuiShipBaseColor[SelectedShipColor];
                    c.GetEntities<Button>()
                         .Where(b => b.Tag.Equals(Tags.ShipSelectLeft) || b.Tag.Equals(Tags.ShipSelectRight) || b.Tag.Equals(Tags.StartGame)).ToList()
                         .ForEach(b =>
                         {
                             b.BaseColor = GuiShipBaseColor[SelectedShipColor];
                             b.FocusColor = GuiShipFocusColor[SelectedShipColor];
                         });
                }
            });
    }
}
