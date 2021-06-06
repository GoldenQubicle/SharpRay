using System.Numerics;

namespace SharpRay
{
    public interface IUIEvent
    {
        UIComponent UIComponent { get; init; }
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

    public interface IEditEvent : IUIEvent
    {
        void Undo();
        void Redo();
    }

    public struct ScaleEdit : IEditEvent
    {
        public UIComponent UIComponent { get; init; }
        public float Start { get; init; }
        public float End { get; init; }
        public void Undo() => UIComponent.Scale = Start;
        public void Redo() => UIComponent.Scale = End;
    }

    public struct TranslateEdit : IEditEvent
    {
        public UIComponent UIComponent { get; init; }
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public void Undo() => UIComponent.Position = Start;
        public void Redo() => UIComponent.Position = End;
    }

    public struct DeleteEdit : IEditEvent
    {
        public UIComponent UIComponent { get; init; }
        public void Undo() => Program.UIComponents.Add(UIComponent);
        public void Redo() => Program.UIComponents.Remove(UIComponent);
    }
}
