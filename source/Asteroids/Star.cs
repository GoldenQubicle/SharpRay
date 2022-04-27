using Raylib_cs;
using SharpRay.Entities;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
using SharpRay.Components;
using System;

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
            texture = GetTexture2D("supersmallstar");
            scaleEasing = new Easing(Easings.EaseBounceInOut, 1870, isRepeated: true);
            offset = new Vector2(texture.width / 2, texture.height / 2);
        }

        public override void Update(double deltaTime)
        {
            scaleEasing.Update(deltaTime);
        }


        public override void Render()
        {
            scale = MapRange(scaleEasing.GetValue(), 0f, 1f, -0.15f, 0.15f);
            BeginBlendMode(BlendMode.BLEND_ADDITIVE);
            DrawTextureEx(texture, Position - offset * scale, 0, scale, Color.GOLD);
            DrawTextureEx(texture, Position - new Vector2(0, texture.height * 0.7f) * scale, 45, scale, Color.GOLD);
            EndBlendMode();
        }
    }
}
