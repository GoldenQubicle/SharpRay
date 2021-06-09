using System;

namespace SharpRay
{
    public interface IUIEventEmitter { Action<IUIEvent> EmitUIEvent { get; set; } }

    public interface IAudioEventEmitter { Action<IAudioEvent> EmitAdudioEvent { get; set; } }
}
