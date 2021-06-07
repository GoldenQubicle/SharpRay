namespace SharpRay
{
    public struct KeyPressed : IKeyBoardEvent { public char Char { get; init; } }

    public struct KeyUp : IKeyBoardEvent { }

    public struct KeyRight : IKeyBoardEvent { }

    public struct KeyDown : IKeyBoardEvent { }

    public struct KeyLeft : IKeyBoardEvent { }

    public struct KeyDelete : IKeyBoardEvent { }

    public struct KeyUndo : IKeyBoardEvent { }

    public struct KeyRedo : IKeyBoardEvent { }

}
