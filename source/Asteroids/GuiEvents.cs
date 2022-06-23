namespace Asteroids
{
    public class GuiEvents
    {
        public struct ChangeShipType : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
            public int ShipType { get; init; }
        }

        public struct ChangeShipColor : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
            public string ShipColor { get; init; }
        }

        public struct GameStart : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
        }

        public struct NextLevel : IGuiEvent
        {
            public GuiEntity GuiEntity { get ; init; }
        }

        public struct ContinueWithLevel : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
        }

        public struct SelectShip : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
        }

        public struct ShowCredits : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
        }
    }
}
