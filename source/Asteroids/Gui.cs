namespace Asteroids
{
    public static class Gui
    {
        public static class Tags
        {
            public const string ShipSelection = nameof(ShipSelection);
            public const string ScoreOverlay = nameof(ScoreOverlay);
            public const string Health = nameof(Health);
            public const string Score = nameof(Score);
            public const string StartGame = nameof(StartGame);
            public const string ShipSelectRight = nameof(ShipSelectRight);
            public const string ShipSelectLeft = nameof(ShipSelectLeft);
            public const string Notification = nameof(Notification);
        }

        public static string PlayerLifeIcon(int n) => $"PlayerLife{n}";
        public static string GetScoreString(int s) => $"Score : {s}";
        public static string GetHealthString(int h) => $"Health : {h}";

        //ship colors
        public const string Blue = "blue";
        public const string Red = "red";
        public const string Green = "green";
        public const string Orange = "orange";

        public static Dictionary<string, Color> GuiShipBaseColor = new()
        {
            { Blue, Color.DARKBLUE },
            { Red, Color.MAROON },
            { Green, Color.LIME },
            { Orange, new Color(200, 100, 0, 255) },
        };

        private static Dictionary<string, Color> GuiShipFocusColor = new()
        {
            { Blue, Color.BLUE },
            { Red, Color.RED },
            { Green, Color.GREEN },
            { Orange, Color.ORANGE },
        };

        public static GuiContainer CreatePickUpNotification() =>
            GuiContainerBuilder.CreateNew(isVisible: false, tag: Tags.Notification, renderLayer: RlGuiScoreOverlay)
                .AddChildren(
                    new Label
                    {
                        Tag = Tags.Notification,
                        Size = new Vector2 (200, 75),
                        Position = new Vector2(WindowWidth/2, WindowHeight/2),
                        FillColor = Color.SKYBLUE,
                        TextColor = Color.RAYWHITE,
                        FontSize = 16,
                        Margins = new Vector2(8, 16)
                    },
                    new Label
                    {
                        Size = new Vector2(150, 30),
                        Position = new Vector2(WindowWidth/2, WindowHeight/2 + 35),
                        FillColor = Color.BLANK,
                        TextColor = Color.RAYWHITE,
                        FontSize = 10,
                        Text = "Press space bar to continue" ,
                        HasOutlines = false,
                        Margins = new Vector2(5, 0)
                    }
                );

        public static GuiContainer CreateScoreOverLay()
        {
            //create container 
            var container = GuiContainerBuilder.CreateNew(isVisible: false, tag: Tags.ScoreOverlay, renderLayer: RlGuiScoreOverlay);

            //add score & health displays
            container.AddChildren(
                new Label
                {
                    Tag = Tags.Score,
                    Position = new Vector2(WindowWidth - 200, 35),
                    Size = new Vector2(200, 50),
                    Text = GetScoreString(Score),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    Margins = new Vector2(10, 10)
                },
                new Label
                {
                    Tag = Tags.Health,
                    Position = new Vector2(WindowWidth - 500, 35),
                    Size = new Vector2(170, 50),
                    Text = GetHealthString(MaxHealth),
                    TextColor = Color.RAYWHITE,
                    FillColor = Color.BLANK,
                    FontSize = 32,
                    Margins = new Vector2(10, 10)
                });

            //add player life icons
            var icon = GetTexture2D(shipsIcons[ShipType][ShipColor]);

            for (var i = 1; i <= PlayerLifes; i++)
            {
                var pos = new Vector2(icon.width + (i * icon.width * 1.5f), 10);
                container.AddChildren(
                     new ImageTexture(icon, Color.WHITE)
                     {
                         Tag = PlayerLifeIcon(i),
                         Position = pos,
                     });
            }

            return container;
        }

        private static GuiContainer CreateShipSelectionMenu() =>
           GuiContainerBuilder.CreateNew(tag: Tags.ShipSelection, renderLayer: RlGuiShipSelection).AddChildren(
               new Label
               {
                   Text = "Meteor Madness",
                   TextColor = Color.YELLOW,
                   FillColor = GuiShipBaseColor[ShipColor],
                   FontSize = 45,
                   Position = new Vector2((WindowWidth / 2), WindowHeight / 8),
                   Size = new Vector2(400, 100),
                   Margins = new Vector2(35, 30),
               },
               new ImageTexture(GetTexture2D(ships[ShipType][ShipColor]), Color.WHITE)
               {
                   Position = new Vector2(WindowWidth / 2, WindowHeight / 2) -
                   new Vector2(GetTexture2D(ships[ShipType][ShipColor]).width / 2, GetTexture2D(ships[ShipType][ShipColor]).height / 2) // rather stupid tbh
               },
               new Button
               {
                   Tag = Tags.ShipSelectLeft,
                   Position = new Vector2(WindowWidth * .2f, WindowHeight / 2),
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
                   Position = new Vector2(WindowWidth * .8f, WindowHeight / 2),
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
                   Margins = new Vector2(28, 15),
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
                   StartGame();
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
                            b.BaseColor = GuiShipBaseColor[ShipColor]; ;
                            b.FocusColor = GuiShipFocusColor[ShipColor]; ;
                        });
               }
           });
    }
}
