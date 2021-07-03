using SharpRay.Collision;
using SharpRay.Eventing;
using System;

namespace SharpRay.Entities
{
    public abstract class GameEntity : Entity, IHasCollider, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }
        public ICollider Collider { get; set; }
    }
}
