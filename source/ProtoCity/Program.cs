global using SharpRay.Core;
global using SharpRay.Gui;
global using static SharpRay.Core.Application;
global using static Raylib_cs.Raylib;
global using Raylib_cs;
global using SharpRay.Eventing;
global using System.Numerics;
global using SharpRay.Entities;
global using System.Collections.Generic;

namespace ProtoCity
{
    public static class Program
    {
        internal static int WindowWidth = 1080;
        internal static int WindowHeight = 720;
        internal static int CellSize = 10;

        public static void Main(string[] args)
        {
            Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

            var background = GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, Color.Beige, Color.Brown);
            AddEntity(new ImageTexture(background, Color.Gray));
            AddEntity(new GridHandler(CellSize));
            AddEntity(GuiContainerBuilder.CreateNew()
                .AddChildren(
                    new Button
                    {
                        Position = new Vector2(100, 35),
                        Size = new Vector2(175, 25),
                        TextOffSet = new Vector2(12, 5),
                        Text = "Transit Tool : Inactive",
                        OnMouseLeftClick = b => new TransitToolToggle { GuiEntity = b }
                    },
                    new Button
                    {
                        Position = new Vector2(300, 35),
                        Size = new Vector2(175, 25),
                        TextOffSet = new Vector2(12, 5),
                        Text = "Brush Tool : Inactive",
                        OnMouseLeftClick = b => new BrushToolToggle { GuiEntity = b }
                    },
                    // NOTE order of gui entities within container matters
                    // tooling needs to come after buttons so button can handle mouse events first, and mark event as handled
                    // otherwise tooling will be active on the same mouse click event, drawing over the button and other dumb stuff
                    new TransitTool(),
                    new BrushTool())
                 .OnGuiEvent((e, c) =>
                 {
                     var tt = c.GetEntity<TransitTool>();
                     var bt = c.GetEntity<BrushTool>();

                     if (e is TransitToolToggle ttt && !bt.IsActive)
                     {
                         tt.IsActive = !tt.IsActive;
                         var b = ttt.GuiEntity as Button;
                         b.Text = $"Transit Tool :  {(tt.IsActive ? "Active" : "Inactive")}";
                     }

                     if (e is BrushToolToggle btt && !tt.IsActive)
                     {
                         bt.IsActive = !bt.IsActive;
                         var b = btt.GuiEntity as Button;
                         b.Text = $"Brush Tool :  {(bt.IsActive ? "Active" : "Inactive")}";
                     }

                 }));

            SetKeyBoardEventAction(OnKeyBoardEvent);

            Run();
        }

        private static void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyPressed kp && kp.KeyboardKey == KeyboardKey.Space)
            {
                GetEntity<GuiContainer>().GetEntity<TransitTool>().Clear();
                GetEntity<GuiContainer>().GetEntity<BrushTool>().Clear();
                GetEntity<GridHandler>().Clear();
            }
        }
    }
}
