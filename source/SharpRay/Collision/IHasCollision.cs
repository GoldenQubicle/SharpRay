using SharpRay.Entities;

namespace SharpRay.Collision
{
    public interface IHasCollision
    {
        void OnCollision(IHasCollider e);
    }
}
