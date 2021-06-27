using System;

namespace SharpRay
{
    public interface IEventEmitter<TEvent> where TEvent : IEvent { Action<TEvent> EmitEvent { get; set; } }

}
