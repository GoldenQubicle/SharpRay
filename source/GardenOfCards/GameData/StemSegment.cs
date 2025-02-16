namespace GardenOfCards.GameData
{
    internal class StemSegment
    {
        public Vector2 Start { get; private set; }
        public Vector2 End => Start + Vector2.Transform(new(0, -Height), Matrix3x2.CreateRotation(Rotation));
        public float Height { get; private set; }
        public float BaseWidth { get; private set; }
        public float TopWidth { get; private set; }
        public float Rotation { get; private set; }

        private static Texture2D _texture2D;

        private Vector2[] Vertices => new List<Vector2>
        {
            new(TopWidth, -Height),
            new(-TopWidth, -Height),
            new(-BaseWidth, 0),
            new(BaseWidth, 0),
            new(TopWidth, -Height),
        }.Select(p => Vector2.Transform(p, Matrix3x2.CreateRotation(Rotation))).ToArray();

        private List<StemSegment> _segments = new();

        public static Texture2D Texture2d
        {
            get
            {
                if (!_texture2D.Equals(default(Texture2D))) return _texture2D;

                var mask = GenImageColor(200, 200, Color.Brown);
                var image = GenImageColor(200, 200, Color.DarkGreen);

                ImageAlphaMask(ref mask, GenImageCellular(200, 200, 20));
                ImageDraw(ref image, mask, new(0, 0, 200, 200), new(0, 0, 200, 200), Color.Lime);

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

        public StemSegment(Vector2 start, int stat)
        {
            Start = start;
            Height = MapRange(stat, Game.MinStat, Game.MaxStat, 50, 125);
            BaseWidth = MapRange(stat, Game.MinStat, Game.MaxStat, 10, 25);
            TopWidth = MapRange(stat, Game.MinStat, Game.MaxStat, 2, 10);
            Rotation = MapRange(stat, Game.MinStat, Game.MaxStat, -.25f, .25f);
        }

        public StemSegment(StemSegment segment)
        {
            Start = segment.End;
            Height = (int)(segment.Height * .8f);
            BaseWidth = segment.TopWidth;
            TopWidth = segment.TopWidth <= 0 ? 0 : (int)(segment.TopWidth * .8f);
            Rotation = segment.Rotation + (GetRandomValue(-30, 30) * DEG2RAD);
         }


        public void Render()
        {

            //Height = (int)-p[0].Y;
            
            //NOTE: draw texture poly is no longer available in raylib 5, for some fucking reason
            //DrawTexturePoly(Texture2d, Start, Vertices, _uv, Vertices.Length, Color.WHITE);

            _segments.ForEach(s => s.Render());

            DrawCircleV(Start, 2, Color.Purple);
            DrawCircleV(Start + Vertices[0], 2, Color.Yellow);
            DrawCircleV(Start + Vertices[1], 2, Color.Orange);
            DrawCircleV(End, 3, Color.Red);

        }



        private StemSegment GetLast() => _segments.Count == 0 ? this : _segments.Last();


        public void OnTurnEnd()
        {
            _segments.Add(new(GetLast()));


            if (GroundKeeper.CurrentTurn.Number == 3)
            {
                ScaleWidth(1.25f);
                ScaleHeight(1.35f);
            }
        }

        private void ScaleWidth(float scale)
        {
            TopWidth *= scale;
            BaseWidth *= scale;
            var nextStart = GetAdjustedStart(this);
            _segments.ForEach(s =>
            {
                s.TopWidth *= scale;
                s.BaseWidth *= scale;
                s.Start = nextStart;
                nextStart = GetAdjustedStart(s);
            });
        }

        private void ScaleHeight(float scale)
        {
            Height *= scale;
            var nextStart = GetAdjustedStart(this);
            _segments.ForEach(s =>
            {
                s.Height *= scale;
                s.Start = nextStart;
                nextStart = GetAdjustedStart(s);
            });
        }

        private Vector2 GetAdjustedStart(StemSegment s) =>
            Vector2.Lerp(s.Start + s.Vertices[0], s.Start + s.Vertices[1], .5f);
    }
}
