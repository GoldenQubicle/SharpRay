using SharpRay.Eventing;
using SharpRay.Entities;

namespace ProtoCity
{
    public class TransitToolToggle : IGuiEvent
    {
        public GuiEntity GuiComponent { get; init; }
    }
}
