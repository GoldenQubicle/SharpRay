namespace SharpRay.Interfaces
{
    public interface ICollider
    {
        public Vector2 Position { get; set; }
        bool Overlaps(ICollider collider);
        bool ContainsPoint(Vector2 point);
        void Render();
    }
}
