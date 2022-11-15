using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeUp : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

}
