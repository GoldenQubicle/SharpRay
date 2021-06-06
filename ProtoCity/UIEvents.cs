using System.Numerics;

namespace ProtoCity
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

    public interface IEditEvent : IUIEvent
    {
        void Undo();
        void Redo();
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
