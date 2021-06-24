using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticleFood : GameEntity
    {
        public override void Render()
        {
            DrawRectangleRounded(Collider, .5f, 1, Color.LIME);
            DrawRectangleRoundedLines(Collider, .5f, 2, 1, Color.GREEN);
        }
    }
}
