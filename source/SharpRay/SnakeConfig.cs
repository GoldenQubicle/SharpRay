namespace SharpRay
{
    public static class SnakeConfig
    {
        public const int WindowHeight = 480;
        public const int WindowWidth = 800;
        public const int CellSize = 40;
        public const int HeadSize = 36;
        public const int SegmentSize = 30;
        public const int FoodSize = 25;
        public const int PoopSize = 20;
        public const int LocomotionInterval= 550;
        public const double MinFoodSpawnInterval = 1000d;
        public const double MaxFoodSpawnInterval = 2500d;
        public const int StartMenuWidth = 300;
        public const string ButtonPushSound = @"Tiny Button Push-SoundBible.com-513260752.wav";
        public const string FootStepSound = @"Fotstep_Carpet_Left.wav";
        public const string SnakeGrow = @"maximize_006.wav";
        public const string SnakeShrink = @"minimize_006.wav";
        public const string FoodSpawn = @"drop_004.wav";
        public const string PoopSpawn = @"drop_004_rev.wav";
        public const string GameOver = @"error_006.wav";
    }
}
