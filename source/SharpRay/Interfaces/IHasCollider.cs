namespace SharpRay.Interfaces
{
    public interface IHasCollider
    {
        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public ICollider Collider { get; }
    }
}
