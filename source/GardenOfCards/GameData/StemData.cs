using Raylib_cs;

namespace GardenOfCards.GameData
{
    internal record StemData
    {
        public Vector2 Start { get; }
        public Vector2 End => Start + Vector2.Transform(new(0, -Height), Matrix3x2.CreateRotation(Rotation));
        public int Height { get; private set; }
        public int BaseWidth { get; private set; }
        public int TopWidth { get; private set; }
        public float Rotation { get; private set; }

        public List<StemData> Segments = new();

        private static Texture2D _texture2D;
        private Vector2[] Points  => new List<Vector2>
        {
            new (TopWidth, -Height),
            new (-TopWidth, -Height),
            new (-BaseWidth, 0),
            new (BaseWidth,   0),
            new (TopWidth, -Height),
        }.Select(p => Vector2.Transform(p, Matrix3x2.CreateRotation(Rotation))).ToArray();

        public static Texture2D Texture2d
        {
            get
            {
                if (!_texture2D.Equals(default(Texture2D))) return _texture2D;

                var mask = GenImageColor(200, 200, Color.BROWN);
                var image = GenImageColor(200, 200, Color.DARKGREEN);

                ImageAlphaMask(ref mask, GenImageCellular(200, 200, 20));
                ImageDraw(ref image, mask, new(0, 0, 200, 200), new(0, 0, 200, 200), Color.LIME);

                _texture2D = LoadTextureFromImage(image);

                UnloadImage(mask);
                UnloadImage(image);

                return _texture2D;
            }
        }

        private static Vector2[] _uv => new Vector2[]
        {
            new(0,0), new (-1,0), new (-1,1),new (0,1)
        };

        public StemData(Vector2 start, int stat)
        {
            Start = start;
            Height = MapRange(stat, Game.MinStat, Game.MaxStat, 50, 125);
            BaseWidth = MapRange(stat, Game.MinStat, Game.MaxStat, 10, 25);
            TopWidth = MapRange(stat, Game.MinStat, Game.MaxStat, 2, 10);
            Rotation = MapRange(stat, Game.MinStat, Game.MaxStat, -.25f, .25f);


            Segments.Add(this);
        }

        public StemData(StemData data)
        {
            Start = data.End;
        }


        public void Render()
        {


            DrawTexturePoly(Texture2d, Start, Points, _uv, Points.Length, Color.WHITE);

            


            DrawCircleV(Start, 5, Color.PURPLE);
            DrawCircleV(End, 3, Color.ORANGE);
        }


    }
}
