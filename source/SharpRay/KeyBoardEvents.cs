namespace SharpRay
{
    public interface IKeyBoardEvent { }

    public struct KeyDelete : IKeyBoardEvent { }

    public struct KeyUndo : IKeyBoardEvent { }

    public struct KeyRedo : IKeyBoardEvent { }

    public struct KeyPressed : IKeyBoardEvent
    {
        public char Char { get; init; }
    }
}
