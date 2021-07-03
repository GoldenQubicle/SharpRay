using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;

namespace SharpRay.Gui
{
    public struct GuiEvent : IGuiEvent
    {
        public GuiEntity GuiComponent { get; init; }
    }


    #region undo & redo ui events
    public interface IHasUndoRedo
    {
        void Undo();
        void Redo();
    }

    public struct ScaleEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => GuiComponent.Scale = Start;
        public void Redo() => GuiComponent.Scale = End;
    }

    public struct TranslateEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => GuiComponent.Position = Start;
        public void Redo() => GuiComponent.Position = End;
    }

    public struct DeleteEdit : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }
        public void Undo() => throw new NotImplementedException();
        public void Redo() => throw new NotImplementedException();
    }

    #endregion
}
