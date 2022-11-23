namespace GardenOfCards
{
    public static class Game
    {
        internal const int WindowWidth = 1080;
        internal const int WindowHeight = 720;


        static async Task Main(string[] args)
        {
            Initialize(new()
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                BackGroundColor = Color.BEIGE,
                ShowFPS = true,
                DoEventLogging = false
            });

            //AddEntity(new Plant());
            GroundKeeper.OnTurnStart(new(4));

            Run();
        }
    }
}

