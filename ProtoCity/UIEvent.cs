using System;
using System.Numerics;

namespace ProtoCity
{
    public interface UIEvent { }
    
    public interface IEditEvent : UIEvent
    {
        UIComponent UIComponent { get; init; }
        void Undo();
        void Redo ();
    }

    public struct TranslateEdit : IEditEvent
    {
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public UIComponent UIComponent { get; init; }
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
