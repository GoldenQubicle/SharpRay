using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeLeft : IKeyBoardEvent 
    {
        public bool IsHandled { get; set; }
    }

}
