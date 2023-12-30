namespace TerribleTetris
{
	internal record GridData(int Rows, int Cols, int CellSize)
	{
		public int Height = Rows * CellSize;
		public int Width = Cols * CellSize;
		public Vector2 Position => new(( WindowWidth - Width ) / 2, ( WindowHeight - Height ) / 2);

	};
}