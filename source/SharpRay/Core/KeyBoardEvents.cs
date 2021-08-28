using Raylib_cs;
using SharpRay.Eventing;

namespace SharpRay.Core
{
    //general event containing raylib enum
    public struct KeyPressed : IKeyBoardEvent
    {
        public KeyboardKey KeyboardKey { get; init; }
    }

    //up, down, left, right down & release events emitted for both arrow and wasd keys
    public struct KeyUpDown : IKeyBoardEvent { }
    public struct KeyRightDown : IKeyBoardEvent { }
    public struct KeyDownDown : IKeyBoardEvent { }
    public struct KeyLeftDown : IKeyBoardEvent { }
    
    public struct KeyUpReleased : IKeyBoardEvent { }
    public struct KeyRightReleased : IKeyBoardEvent { }
    public struct KeyDownReleased : IKeyBoardEvent { }
    public struct KeyLeftReleased : IKeyBoardEvent { }
    
    //pretty self explanatory
    public struct KeySpaceBarPressed : IKeyBoardEvent { }
    public struct KeyDelete : IKeyBoardEvent { }
    
    //emitted on ctr-z & ctrl-y
    public struct KeyUndo : IKeyBoardEvent { }
    public struct KeyRedo : IKeyBoardEvent { }

}
