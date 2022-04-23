using SharpRay.Eventing;
using SharpRay.Interfaces;
using SharpRay.Listeners;
using System.Numerics;

namespace SharpRay.Entities
{

    public abstract class Entity : IKeyBoardListener, IMouseListener, IHasUpdate, IHasRender
    {
        public Vector2 Size { get; init; }

        public Vector2 Position { get; set; }

        public int RenderLayer { get; set; }
        
        public virtual void Render() { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }

        public virtual void Update(double deltaTime) { }

    }
}
