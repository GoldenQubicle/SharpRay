using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeRight : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

}
