using System.Numerics;

namespace SharpRay.Collision
{
    public interface ICollider
    {
        bool ContainsPoint(Vector2 point);
        bool Overlaps(ICollider collider);
    }
}
