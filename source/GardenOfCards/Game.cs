namespace GardenOfCards
{
    public static class Game
    {
        internal const int WindowWidth = 1080;
        internal const int WindowHeight = 720;

        static async Task Main(string[] args)
        {
            Initialize(new SharpRayConfig
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = Color.BEIGE,
                ShowFPS = true,
                DoEventLogging = false
            });

            AddEntity(new Card
            {
                Size = new (128, 256),
                Position = new (WindowWidth/2, WindowHeight/2),
                Scale = 1,
                CanScale = false,
                ColorDefault = Color.WHITE,
                ColorFocused = Color.RED,
                
            });

            Run();
        }
    }
}

