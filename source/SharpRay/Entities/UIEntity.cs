using System;
using System.Numerics;

namespace SharpRay
{
    /// <summary>
    /// Classes implementing UIEntity can enable mouse focus by overriding ContainsPoint, and calling base.OnMouseEvent
    /// </summary>
    public abstract class UIEntity : Entity, IEventEmitter<IUIEvent>
    {
        public Action<IUIEvent> EmitEvent { get; set; }
        public Func<UIEntity, IUIEvent> OnMouseLeftClick { get; set; }

        public float Scale { get; set; } = 1f;

        public bool HasMouseFocus { get; set; }

        public virtual bool ContainsPoint(Vector2 point) => false;
        public override void OnMouseEvent(IMouseEvent e) => HasMouseFocus = ContainsPoint(e.Position);
    }
}

