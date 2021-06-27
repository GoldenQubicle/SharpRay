using System;

namespace SharpRay
{
    public interface IUIEventListener<TEntity> where TEntity : Entity
    {
        Action<IUIEvent, TEntity> OnUIEventAction { get; set; }
        void OnUIEvent(IUIEvent e);
    }
}
