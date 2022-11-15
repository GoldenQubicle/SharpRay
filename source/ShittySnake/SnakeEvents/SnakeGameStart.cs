using SharpRay.Entities;
using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeGameStart : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}
