using SharpRay.Eventing;
using SharpRay.Gui;
using System;
using System.Numerics;

namespace SharpRay.Entities
{
    /// <summary>
    /// Classes implementing GuiEntity can enable mouse focus by overriding ContainsPoint, and calling base.OnMouseEvent
    /// </summary>
    public abstract class GuiEntity : Entity, IEventEmitter<IGuiEvent>
    {
        public Action<IGuiEvent> EmitEvent { get; set; }
        public Func<GuiEntity, IGuiEvent> OnMouseLeftClick { get; set; }
     

        public float Scale { get; set; } = 1f;

        public bool HasMouseFocus { get; set; }

        public virtual bool ContainsPoint(Vector2 point) => false;
        public override void OnMouseEvent(IMouseEvent e) => HasMouseFocus = ContainsPoint(e.Position);
    }
}

