namespace SharpRay.Core
{
    //general event containing raylib enum
    public class KeyPressed : IKeyBoardEvent
    {
        public KeyboardKey KeyboardKey { get; init; }
        public bool IsHandled { get; set; }

    }

    //up, down, left, right down & release events emitted for both arrow and wasd keys
    public class KeyUpDown : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyRightDown : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyDownDown : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyLeftDown : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

    public class KeyUpReleased : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyRightReleased : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyDownReleased : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyLeftReleased : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

    //pretty self explanatory
    public class KeySpaceBarPressed : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeySpaceBarDown : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyDelete : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

    //emitted on ctr-z & ctrl-y
    public class KeyUndo : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }
    public class KeyRedo : IKeyBoardEvent
    {
        public bool IsHandled { get; set; }
    }

}
