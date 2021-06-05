namespace ProtoCity
{
    public interface IKeyBoardEvent { }

    public struct KeyDelete : IKeyBoardEvent { }

    public struct KeyUndo : IKeyBoardEvent { }

    public struct KeyRedo : IKeyBoardEvent { }
}
