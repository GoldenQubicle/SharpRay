namespace TerribleTetris
{
	internal static class TetrominoController
	{
		private static Tetromino _activeTetromino;

		public static void Push(Tetromino tetromino) => _activeTetromino = tetromino;

		
	}
}
