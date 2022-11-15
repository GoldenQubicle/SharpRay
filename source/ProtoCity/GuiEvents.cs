namespace ProtoCity
{
    public class TransitToolToggle : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }

    public class BrushToolToggle : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}
