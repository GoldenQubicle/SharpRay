using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticlePoop : GameEntity
    {
        public Color Color { get; init; }
        public override void Render(double deltaTime)
        {
            DrawRectangleRec(Collider, Color);
        }
    }
}
