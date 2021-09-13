using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;
using static SharpRay.Core.Application;

namespace SharpRay.Gui
{
    public class GuiEvent : IGuiEvent
    {
        public GuiEntity GuiComponent { get; init; }
    }

    #region undo & redo ui events
    public interface IHasUndoRedo
    {
        void Undo();
        void Redo();
    }

    public class ScaleEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => GuiComponent.Scale = Start;
        public void Redo() => GuiComponent.Scale = End;
    }

    public class TranslateEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => GuiComponent.Position = Start;
        public void Redo() => GuiComponent.Position = End;
    }

    public class DeleteEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public void Undo() => AddEntity(GuiComponent);
        public void Redo() => RemoveEntity(GuiComponent);
    }

    #endregion
}
