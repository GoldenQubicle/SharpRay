using System;

namespace SharpRay
{
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
