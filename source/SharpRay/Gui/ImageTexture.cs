﻿namespace SharpRay.Gui
{
    public sealed class ImageTexture : GuiEntity
    {
        public Texture2D Texture2D { get; set; }
        public Color Color { get; set; }
        public bool HasOutline { get; set; }
        public ImageTexture(Image image, Color color)
        {
            Color = color;
            Texture2D = LoadTextureFromImage(image);
            UnloadImage(image);
        }

        public ImageTexture(Texture2D texture, Color color)
        {
            Color = color;
            Texture2D = texture;
        }

        public override void Render()
        {
            if(HasOutline)
                DrawRectangleLines((int)Position.X, (int)Position.Y, Texture2D.Width, Texture2D.Height, Color);
            DrawTexture(Texture2D, (int)Position.X, (int)Position.Y, Color);
        }
    }
}

