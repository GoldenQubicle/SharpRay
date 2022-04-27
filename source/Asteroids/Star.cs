using Raylib_cs;
using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
using SharpRay.Components;

namespace Asteroids
{
    public class Star : GameEntity
    {
        private Texture2D texture;
        private float scale;
        private Easing scaleEasing;
        private Vector2 offset;
        public Star(Vector2 pos)
        {
            Position = pos;
            texture = GetTexture2D("starTexture");
            scaleEasing = new Easing(Easings.EaseBackInOut, 1870, isRepeated: true);
            offset = new Vector2(texture.width / 2, texture.height / 2);
        }

        public override void Update(double deltaTime)
        {
            scaleEasing.Update(deltaTime);
        }


        public override void Render()
        {
            scale = MapRange(scaleEasing.GetValue(), 0f, 1f, 0.25f, 0.55f);
            DrawTextureEx(texture, Position - offset * scale, 0, scale, Color.GOLD);
            
        }
    }
}
