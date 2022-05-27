namespace Asteroids
{
    public class StarField : GameEntity
    {
        private struct Star
        {
            public Vector2 Position { get; init; }
            public bool IsDiagonal { get; init; }
            public Color Color { get; init; }
            public Easing Easing { get; init; }
            public float ScaleRange { private get; init; }

            public float GetScale() =>
                MapRange(Easing.GetValue(), 0f, 1f, -ScaleRange, ScaleRange);
        }

        private Texture2D texture;
        private Vector2 textureOffset;
        private List<Star> stars = new();

        private List<Color> starColors = new()
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

        private List<Func<float, float, float, float, float>> scaleEasings = new()
        {
            Easings.EaseBounceInOut,
            Easings.EaseBackInOut,
        };

        public StarField()
        {
            RenderLayer = Game.RlBackground;
            texture = GetTexture2D(Game.starTexture);
            textureOffset = new Vector2(texture.width / 2, texture.height / 2);
            Generate();
        }

        public void Generate()
        {
            stars.Clear();
            stars = Enumerable.Range(0, 75)
               .Select(s => new Star
               {
                   Position = new Vector2(GetRandomValue(0, Game.WindowWidth), GetRandomValue(0, Game.WindowHeight)),
                   Easing = new Easing(scaleEasings[GetRandomValue(0, 1)], GetRandomValue(3500, 7500), isRepeated: true),
                   ScaleRange = GetRandomValue(3, 17) / 100f,
                   Color = starColors[GetRandomValue(0, starColors.Count - 1)],
                   IsDiagonal = GetRandomValue(0, 1) == 1,
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
            foreach (var star in stars)
            {
                var scale = star.GetScale();

                BeginBlendMode(BlendMode.BLEND_ADDITIVE);

                if (star.IsDiagonal)
                    DrawTextureEx(texture, star.Position - new Vector2(0, texture.height * 0.7f) * scale, 45, scale, star.Color);
                else
                    DrawTextureEx(texture, star.Position - textureOffset * scale, 0, scale, star.Color);

                EndBlendMode();
            }
        }
    }
}
