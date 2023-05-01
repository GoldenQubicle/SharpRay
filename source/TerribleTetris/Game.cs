namespace TerribleTetris
{
	public static class Game
	{
		public static int WindowHeight => 480;

		public static int WindowWidth => 680;

		static void Main(string[ ] args)
		{
			Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight, DoEventLogging = false, ShowFPS = true});
			

			Run( );
		}
	}
}