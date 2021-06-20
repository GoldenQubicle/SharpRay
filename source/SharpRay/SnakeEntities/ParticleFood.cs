using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticleFood : GameEntity
    {
        public Color Color { get; init; }
        public override void Render()
        {
            DrawRectangleRec(Collider, Color.LIME);
            DrawRectangleLinesEx(Collider, 1, Color.GREEN);
        }
    }
}
