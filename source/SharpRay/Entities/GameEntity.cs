using SharpRay.Collision;
using SharpRay.Eventing;
using System;

namespace SharpRay.Entities
{
    public abstract class GameEntity : Entity, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }
    }
}
