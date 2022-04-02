using SharpRay.Entities;
using SharpRay.Eventing;

namespace SnakeEvents
{
    public class SnakeGameStart : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }
}
