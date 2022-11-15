using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeDown : IKeyBoardEvent 
    {
        public bool IsHandled { get; set; }
    }

}
