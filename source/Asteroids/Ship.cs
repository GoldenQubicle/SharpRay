using Raylib_cs;
using SharpRay.Entities;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class Ship : GameEntity
    {
        public override void Render()
        {
            DrawRectangleV(Position, Size, Color.RED);
        }
    }
}
