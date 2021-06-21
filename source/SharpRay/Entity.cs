using Raylib_cs;
using System;
using System.Numerics;

namespace SharpRay
{
    public interface IMouseListener { void OnMouseEvent(IMouseEvent e); }
    public interface IKeyBoardListener { void OnKeyBoardEvent(IKeyBoardEvent e); }
    public interface IUIEventListener
    {
        Action<IUIEvent, Entity> OnUIEventAction { get; set; }
        void OnUIEvent(IUIEvent e);
    }
    public interface IGameEventListener
    {
        Action<IGameEvent, Entity> OnGameEventAction { get; set; }
        void OnGameEvent(IGameEvent e);
    }
    public interface IHasCollision { void OnCollision(GameEntity e); }
    public interface IHasCollider { public Raylib_cs.Rectangle Collider { get; } }

    public abstract class Entity : IKeyBoardListener, IMouseListener
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; init; }

        /// <summary>
        /// Delta time is the interval since last render frame in ticks!
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Render() { }

        public virtual void Update(double deltaTime) { }

        public virtual void OnKeyBoardEvent(IKeyBoardEvent e) { }

        public virtual void OnMouseEvent(IMouseEvent e) { }

    }

    public abstract class GameEntity : Entity, IHasCollider, IEventEmitter<IGameEvent>
    {
        public Raylib_cs.Rectangle Collider
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
        public Action<IGameEvent> EmitEvent { get; set; }
    }
}
