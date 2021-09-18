using SharpRay.Core;
using SharpRay.Gui;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;
using Raylib_cs;
using SharpRay.Eventing;
using System.Numerics;

namespace ProtoCity
{
    public static class Program
    {
        internal static int WindowWidth = 1080;
        internal static int WindowHeight = 720;
        internal static int CellSize = 10;

        public static void Main(string[] args)
        {
            Initialize(new Config { WindowWidth = WindowWidth, WindowHeight = WindowHeight });

            var background = GenImageChecked(WindowWidth, WindowHeight, CellSize, CellSize, Color.BEIGE, Color.BROWN);
            AddEntity(new ImageTexture(background, Color.GRAY));
            AddEntity(new GridHandler(CellSize));
            AddEntity(GuiEntityContainerBuilder.CreateNew()
                .AddChildren(
                    new Button
                    {
                        Position = new Vector2(10, 10),
                        Size = new Vector2(200, 25),
                        Margins = new Vector2(5, 5),
                        FillColor = Color.BLANK,
                        FocusColor = Color.GRAY,
                        TextColor = Color.RAYWHITE,
                        Text = "Transit Tool : Inactive",
                        OnMouseLeftClick = e => new TransitToolToggle { GuiComponent = e }
                    },
                    new Button
                    {
                        Position = new Vector2(250, 10),
                        Size = new Vector2(200, 25),
                        Margins = new Vector2(5, 5),
                        FillColor = Color.BLANK,
                        FocusColor = Color.GRAY,
                        TextColor = Color.RAYWHITE,
                        Text = "Brush Tool : Inactive",
                        OnMouseLeftClick = e => new BrushToolToggle { GuiComponent = e }
                    },
                    // NOTE order of gui entities within container matters
                    // tooling needs to come after buttons so button can handle mouse events first, and mark event as handled
                    // otherwise tooling will be active on the same mouse click event, drawing over the button and other dumb stuff
                    new TransitTool(),
                    new BrushTool()) 
                 .OnGuiEvent((e, c) =>
                 {
                     var tt = c.Get<TransitTool>();
                     var bt = c.Get<BrushTool>();

                     if (e is TransitToolToggle ttt && !bt.IsActive)
                     {
                         tt.IsActive = !tt.IsActive;
                         var b = ttt.GuiComponent as Button;
                         b.Text = $"Transit Tool :  {(tt.IsActive ? "Active" : "Inactive")}";
                     }

                     if (e is BrushToolToggle btt && !tt.IsActive)
                     {
                         bt.IsActive = !bt.IsActive;
                         var b = btt.GuiComponent as Button;
                         b.Text = $"Brush Tool :  {(bt.IsActive ? "Active" : "Inactive")}";
                     }

                 }));

            Run();
        }
    }
}
