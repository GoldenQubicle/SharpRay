using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticlePoop : GameEntity
    {
        public override void Render()
        {
            DrawRectangleRounded(Collider, .5f, 5, Color.DARKBROWN);
            DrawRectangleRoundedLines(Collider, .5f, 5, 2, Color.BROWN);
        }
    }
}
