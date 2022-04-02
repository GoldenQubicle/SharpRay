namespace SharpRay.Eventing
{
    public interface IKeyBoardEvent : IEvent 
    {
        public bool IsHandled { get; set; } 
    }

}
