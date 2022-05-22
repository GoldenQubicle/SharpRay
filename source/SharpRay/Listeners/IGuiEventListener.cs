namespace SharpRay.Listeners
{
    public interface IGuiEventListener<TEntity> where TEntity : Entity
    {
        Action<IGuiEvent, TEntity> OnGuiEventAction { get; set; }
        void OnGuiEvent(IGuiEvent e);
    }
}
