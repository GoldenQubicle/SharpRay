namespace GardenOfCards.Events
{
    internal struct ResetGame : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}