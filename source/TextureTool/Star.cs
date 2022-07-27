using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace TextureTool
{
    public class Star : Entity
    {
        private Vector2 LeftUp = Vector2.Zero;
        private Vector2 CenterUp;
        private Vector2 RightUp;
        private Vector2 RightCenter;
        private Vector2 RightDown;
        private Vector2 DownCenter;
        private Vector2 LeftDown;
        private Vector2 LeftCenter;
        private float innerRadius;
        private float outerRadius;
        private Color Color = Color.WHITE;

        public Star(int size)
        {
            Size = new Vector2(size / 2, size / 2);
            RightUp = new Vector2(size, 0);
            RightDown = new Vector2(size, size);
            LeftDown = new Vector2(0, size);
            CenterUp = new Vector2(size / 2, 0);
            RightCenter = new Vector2(size, size / 2);
            DownCenter = new Vector2(size / 2, size);
            LeftCenter = new Vector2(size / 2, 0);


        }

        public override void Render()
        {
            innerRadius = Size.X - 2;
            outerRadius = Size.X + 2;
            DrawRing(LeftUp, innerRadius, outerRadius, 0, 90, 16, Color);
            DrawRing(RightUp, innerRadius, outerRadius, 0, -90, 16, Color);
            DrawRing(RightDown, innerRadius, outerRadius, 270, 180, 16, Color);
            DrawRing(LeftDown, innerRadius, outerRadius, 180, 90, 16, Color);



        }

    }
}
