using static SharpRay.Core.Application;

namespace SharpRay.Gui
{
    public class GuiEvent : IGuiEvent
    {
        public GuiEntity GuiEntity { get; init; }
    }

    #region undo & redo ui events
    public interface IHasUndoRedo
    {
        void Undo();
        void Redo();
    }

    public class ScaleEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiEntity { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => GuiEntity.Scale = Start;
        public void Redo() => GuiEntity.Scale = End;
    }

    public class TranslateEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiEntity { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => GuiEntity.Position = Start;
        public void Redo() => GuiEntity.Position = End;
    }

    public class DeleteEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiEntity { get; init; }
        public void Undo() => AddEntity(GuiEntity);
        public void Redo() => RemoveEntity(GuiEntity);
    }

    #endregion
}
