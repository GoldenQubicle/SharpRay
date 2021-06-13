using System.Numerics;

namespace SharpRay
{
    public struct StartSnakeGame : IUIEvent { public UIEntity UIComponent { get; init; } }



    #region undo & redo ui events
    public interface IHasUndoRedo
    {
        void Undo();
        void Redo();
    }

    public struct ScaleEdit : IUIEvent, IHasUndoRedo
    {
        public UIEntity UIComponent { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => UIComponent.Scale = Start;
        public void Redo() => UIComponent.Scale = End;
    }

    public struct TranslateEdit : IUIEvent, IHasUndoRedo
    {
        public UIEntity UIComponent { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => UIComponent.Position = Start;
        public void Redo() => UIComponent.Position = End;
    }

    public struct DeleteEdit : IUIEvent, IHasUndoRedo
    {
        public UIEntity UIComponent { get; init; }
        public void Undo() => Program.Entities.Add(UIComponent);
        public void Redo() => Program.Entities.Remove(UIComponent);
    }
    #endregion
}
