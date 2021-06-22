using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class ParticlePoop : GameEntity
    {
        public Color Color { get; init; }
        public override void Render()
        {
            DrawRectangleRounded(Collider, .7f, 5, Color.DARKBROWN);
            DrawRectangleRoundedLines(Collider, .7f, 5, 1, Color.BROWN);
        }
    }
}
