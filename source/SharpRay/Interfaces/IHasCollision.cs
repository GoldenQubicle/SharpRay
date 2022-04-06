using SharpRay.Interfaces;

namespace SharpRay.Collision
{
    public interface IHasCollision
    {
        void OnCollision(IHasCollider e);
    }
}
