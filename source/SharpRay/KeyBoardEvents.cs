namespace SharpRay
{
    public struct KeyPressed : IKeyBoardEvent { public char Char { get; init; } }

    public struct KeyDelete : IKeyBoardEvent { }

    public struct KeyUndo : IKeyBoardEvent { }

    public struct KeyRedo : IKeyBoardEvent { }

}
