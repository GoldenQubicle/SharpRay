namespace ProtoCity
{
    public abstract class UICommand
    {
        public int EntityId { get; init; }
        public abstract void Undo();
        public abstract void Redo();
    }
}
