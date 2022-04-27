using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace TextureTool
{
    public class Star : GameEntity
    {
        private Vector2 LeftUp = Vector2.Zero;
        private Vector2 RightUp;
        private Vector2 RightDown;
        private Vector2 LeftDown;
        private int innerRadius;
        private int outerRadius;

        public Star(int size)
        {
            Size = new Vector2(size/2, size/2);
            RightUp = new Vector2(size, 0);
            RightDown = new Vector2(size, size);
            LeftDown = new Vector2(0, size);

            innerRadius = size/2 - 10;
            outerRadius = size/2 + 10;
        }

        public override void Render()
        {
            DrawRing(LeftUp, innerRadius, outerRadius, 0, 90, 16, Color.WHITE);
            DrawRing(RightUp, innerRadius, outerRadius, 0, -90, 16, Color.WHITE);
            DrawRing(RightDown, innerRadius, outerRadius, 270, 180, 16, Color.WHITE);
            DrawRing(LeftDown, innerRadius, outerRadius, 180, 90, 16, Color.WHITE);

        }

    }
}
