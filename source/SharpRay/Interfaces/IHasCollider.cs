using SharpRay.Collision;

namespace SharpRay.Interfaces
{
    public interface IHasCollider
    {
        public ICollider Collider { get; }
    }
}
