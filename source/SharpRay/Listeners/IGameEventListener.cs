﻿namespace SharpRay.Listeners
{
    public interface IGameEventListener<TEntity> where TEntity : Entity
    {
        Action<IGameEvent, TEntity> OnGameEventAction { get; set; }
        void OnGameEvent(IGameEvent e);
    }
}
