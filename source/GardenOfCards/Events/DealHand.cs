namespace GardenOfCards.Events
{
    internal struct DealHand : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}