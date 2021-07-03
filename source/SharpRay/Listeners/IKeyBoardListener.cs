using SharpRay.Eventing;

namespace SharpRay.Listeners
{
    public interface IKeyBoardListener
    {
        void OnKeyBoardEvent(IKeyBoardEvent e);
    }
}
