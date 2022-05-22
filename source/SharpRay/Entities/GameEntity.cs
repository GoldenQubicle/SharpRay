namespace SharpRay.Entities
{
    public abstract class GameEntity : Entity, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }
    }
}
