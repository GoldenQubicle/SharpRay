namespace TerribleTetris.GameMode;

internal class PlayMode : IGameMode
{
	public PatternData PatternData { get; set; }

	public void OnStart(GridData gridData)
	{
		AddEntity(new Grid(gridData));
		DropTime = 750;
		TetrominoStack.Clear( );
		PatternData.Shapes.Reverse( );
		PatternData.Shapes.ForEach(s => TetrominoStack.Push(new Tetromino(s.Shape, Rotation.Up, ( gridData.Cols - Tetromino.BoundingBoxSize(s.Shape) ) / 2, gridData.CellSize)));
		AddEntity(new Pattern(PatternData, gridData));
		SpawnTetromino( );
	}

	public void OnGameEvent(IGameEvent e)
	{
		if (e is TetrominoLocked tl)
		{
			GetEntity<Grid>( ).LockCells(tl);
			PatternData.Placed.Add(tl);

			if (IsAboveGrid(tl) || TetrominoStack.Count == 0)
			{
				Print($"Game Over!");
				return;
			}

			SpawnTetromino( );
		}
	}

	public IGameMode NextMode(IGameMode nextGameMode)
	{
		return nextGameMode;
	}
}