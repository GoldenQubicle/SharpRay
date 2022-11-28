namespace GardenOfCards.Events
{
    internal struct EndTurn : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}