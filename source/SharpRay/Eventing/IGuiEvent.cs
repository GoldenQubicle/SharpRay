using SharpRay.Entities;

namespace SharpRay.Eventing
{
    public interface IGuiEvent : IEvent { GuiEntity GuiEntity{ get; init; } }

}
