using SharpRay.Eventing;

namespace SharpRay.Core
{

    public struct KeyUpDown :IKeyBoardEvent { }
    public struct KeyRightDown :IKeyBoardEvent { }
    public struct KeyDownDown :IKeyBoardEvent { }
    public struct KeyLeftDown :IKeyBoardEvent { }
    public struct KeyUpReleased : IKeyBoardEvent { }
    public struct KeyRightReleased : IKeyBoardEvent { }
    public struct KeyDownReleased : IKeyBoardEvent { }
    public struct KeyLeftReleased : IKeyBoardEvent { }
    public struct KeySpaceBarPressed : IKeyBoardEvent { }
    public struct KeyPressed : IKeyBoardEvent { public char Char { get; init; } }

    public struct KeyDelete : IKeyBoardEvent { }

    public struct KeyUndo : IKeyBoardEvent { }

    public struct KeyRedo : IKeyBoardEvent { }

}
