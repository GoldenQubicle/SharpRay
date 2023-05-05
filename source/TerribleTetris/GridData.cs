namespace TerribleTetris
{
	internal record GridData(int Rows, int Cols, int CellSize, Color Color1, Color Color2)
	{
		public int Height = Rows * CellSize;
		public int Width = Cols * CellSize;
		public Vector2 Position => new(( Game.WindowWidth - Width ) / 2, ( Game.WindowHeight - Height ) / 2);

	};
}