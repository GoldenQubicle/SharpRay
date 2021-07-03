using SharpRay.Entities;

namespace SharpRay.Eventing
{
    public interface IGuiEvent : IEvent { GuiEntity GuiComponent { get; init; } }

}
