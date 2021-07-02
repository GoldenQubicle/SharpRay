using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public sealed class ImageTexture : UIEntity
    {
        public Texture2D Texture2D { get; private set; }
        public Color Color { get; init; }

        public ImageTexture(Image image, Color color)
        {
            Color = color;
            Texture2D = LoadTextureFromImage(image);
            UnloadImage(image);
        }

        public override void Render()
        {
            DrawTexture(Texture2D, (int)Position.X, (int)Position.Y, Color);
        }
    }
}

