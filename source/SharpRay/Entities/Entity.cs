using SharpRay.Eventing;
using SharpRay.Listeners;
using System.Numerics;

namespace SharpRay.Entities
{

    public abstract class Entity : IKeyBoardListener, IMouseListener
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; init; }
        public virtual void Render() { }

        /// <summary>
        /// Delta time is the interval since last render frame in ticks!
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(double deltaTime) { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }

    }
}
