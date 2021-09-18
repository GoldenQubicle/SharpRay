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
                    new TransitTool()) // note the order of insertion matter as otherwise the click event is not marked as handled just yet
                 .OnGuiEvent((e, c) =>
                 {
                     if (e is TransitToolToggle)
                     {
                         var tt = c.Get<TransitTool>();
                         tt.IsActive = !tt.IsActive;
                         var b = c.Get<Button>();
                         b.Text = $"Transit Tool :  {(tt.IsActive ? "Active" : "Inactive")}";
                     }
                 }));
            AddEntity(new BrushTool());

            Run();
        }
    }
}
