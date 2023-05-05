namespace TerribleTetris
{
	internal static class Game
	{
		internal static int WindowHeight => 480;

		internal static int WindowWidth => 680;

		internal enum Tetromino { I, O, T, J, L, S, Z };

		internal static void Main(string[ ] args)
		{
			Initialize(new SharpRayConfig { WindowWidth = WindowWidth, WindowHeight = WindowHeight, DoEventLogging = false, ShowFPS = true });

			AddEntity(new Grid(new GridData(Rows: 10, Cols: 5, CellSize: 35)));

			Run( );
		}
	}
}