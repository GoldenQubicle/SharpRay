namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 480;

		internal static int WindowWidth => 680;

		internal enum Shape { I, O, T, J, L, S, Z };

		internal static void Main(string[ ] args)
		{
			Initialize(new SharpRayConfig
			{
				WindowWidth = WindowWidth, 
				WindowHeight = WindowHeight, 
				DoEventLogging = false, 
				ShowFPS = true, 
				BackGroundColor = DARKGRAY
			});

			var gridData = new GridData(Rows: 16, Cols: 10, CellSize: 20, Color1: RAYWHITE, Color2: LIGHTGRAY );
			AddEntity(new Grid(gridData));

			var tetrominoData = new TetrominoData(Shape.Z);
			AddEntity(new Tetromino(tetrominoData, gridData));

			Run( );
		}
	}
}