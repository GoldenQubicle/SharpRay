namespace TerribleTetris
{
	internal record PatternData(int Rows, int Cols, List<TetrominoLocked> Shapes)
	{
		public List<TetrominoLocked>? Placed { get; set; } = new();
	}
}
