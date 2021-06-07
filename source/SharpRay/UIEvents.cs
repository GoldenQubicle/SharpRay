﻿using System.Numerics;

namespace SharpRay
{
   
    public struct UIEvent : IUIEvent
    {
        public UIComponent UIComponent { get; init; }
    }

    public struct ToggleTimer : IUIEvent
    {
        public UIComponent UIComponent { get; init; }
        public bool IsPaused { get; init; }
    }

    public struct RectangleLeftClick : IUIEvent
    {
        public UIComponent UIComponent { get; init; }
    }

    public interface IHasUndoRedo 
    {
        void Undo();
        void Redo();
    }

    public struct ScaleEdit : IUIEvent, IHasUndoRedo
    {
        public UIComponent UIComponent { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => UIComponent.Scale = Start;
        public void Redo() => UIComponent.Scale = End;
    }

    public struct TranslateEdit : IUIEvent, IHasUndoRedo
    {
        public UIComponent UIComponent { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => UIComponent.Position = Start;
        public void Redo() => UIComponent.Position = End;
    }

    public struct DeleteEdit : IUIEvent, IHasUndoRedo
    {
        public UIComponent UIComponent { get; init; }
        public void Undo() => Program.UIComponents.Add(UIComponent);
        public void Redo() => Program.UIComponents.Remove(UIComponent);
    }
}
