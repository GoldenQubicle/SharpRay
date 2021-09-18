using SharpRay.Eventing;
using SharpRay.Entities;

namespace ProtoCity
{
    public class TransitToolToggle : IGuiEvent
    {
        public GuiEntity GuiComponent { get; init; }
    }

    public class BrushToolToggle : IGuiEvent
    {
        public GuiEntity GuiComponent { get; init; }
    }
}
