using System.Numerics;

namespace SharpRay.Interfaces
{
    public interface ICollider
    {
        bool Overlaps(ICollider collider);
        bool ContainsPoint(Vector2 point);
        void Render();
    }
}
