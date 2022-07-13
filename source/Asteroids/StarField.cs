namespace Asteroids
{
    public class StarField : Entity, IHasUpdate, IHasRender
    {
        private struct Star
        {
            public Vector2 Position { get; init; }
            public Easing Easing { get; init; }
            public Color Color { get; init; }
            public bool IsDiagonal { get; init; }
            public float ScaleRange { private get; init; }
            public float GetScale() =>
                MapRange(Easing.GetValue(), 0f, 1f, -ScaleRange, ScaleRange);
        }

        private Texture2D starTexture;
        private Texture2D backGroundTexture;
        private Vector2 starTextureOffset;
        private List<Star> stars = new();

        private readonly List<Color> colors = new()
        {
            Color.YELLOW,
            Color.GOLD,
            Color.ORANGE,
            Color.PINK,
            Color.PURPLE,
            Color.DARKPURPLE,
            Color.DARKBLUE,
            Color.DARKGRAY,
            Color.MAGENTA
        };

        private readonly List<Func<float, float, float, float, float>> scaleEasings = new()
        {
            Easings.EaseBounceInOut,
            Easings.EaseBackInOut,
        };

        public StarField()
        {
            RenderLayer = RlBackground;
            starTexture = GetTexture2D(Assets.starTexture);
            starTextureOffset = new Vector2(starTexture.width / 2, starTexture.height / 2);
            Generate();
        }

        public void Generate()
        {
            // Create the background image
            var c1 = GenImageCellular(WindowWidth, WindowHeight, WindowWidth / 3);
            var c2 = GenImageCellular(WindowWidth, WindowHeight, WindowWidth / 6);
            
            //NOTE order does matter! 
            ImageColorInvert(ref c2);
            ImageColorTint(ref c2, Color.MAGENTA);
            ImageColorContrast(ref c2, 20);
            ImageColorContrast(ref c1, -20);
            ImageAlphaMask(ref c1, c2);
            ImageColorTint(ref c1, Color.DARKBLUE);
            backGroundTexture = LoadTextureFromImage(c1);
            UnloadImage(c1);
            UnloadImage(c2);

            stars.Clear();
            stars = Enumerable.Range(0, 75)
               .Select(s => new Star
               {
                   Position = new Vector2(GetRandomValue(0, WindowWidth), GetRandomValue(0, WindowHeight)),
                   Easing = new Easing(scaleEasings[GetRandomValue(0, 1)], GetRandomValue(3500, 7500), isRepeated: true),
                   Color = ColorAlpha(colors[GetRandomValue(0, colors.Count - 1)], GetRandomValue(20, 90)/100f),
                   IsDiagonal = GetRandomValue(0, 1) == 1,
                   ScaleRange = GetRandomValue(3, 17) / 100f,
               }).ToList();

            foreach (var star in stars)
                star.Easing.SetElapsedTime(GetRandomValue(3500, 7500));
        }

        public override void Update(double deltaTime)
        {
            foreach (var star in stars)
                star.Easing.Update(deltaTime);
        }

        public override void Render()
        {
            DrawTexture(backGroundTexture, 0, 0, Color.WHITE);
            foreach (var star in stars)
            {
                var scale = star.GetScale();

                BeginBlendMode(BlendMode.BLEND_ADDITIVE);

                if (star.IsDiagonal)
                    DrawTextureEx(starTexture, star.Position - new Vector2(0, starTexture.height * 0.7f) * scale, 45, scale, star.Color);
                else
                    DrawTextureEx(starTexture, star.Position - starTextureOffset * scale, 0, scale, star.Color);

                EndBlendMode();
            }
        }
    }
}
