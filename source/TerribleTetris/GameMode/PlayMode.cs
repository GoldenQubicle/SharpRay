

namespace TerribleTetris.GameMode;

internal class PlayMode : IGameMode
{
	public GridData GridData { get; set; }
	public PatternData PatternData { get; set; }

	public PlayMode(string fileName = "")
	{
		if (!string.IsNullOrEmpty(fileName))
		{
			var json = File.ReadAllText(Path.Combine(AssestsFolder, fileName));
			PatternData = JsonSerializer.Deserialize<PatternData>(json, GetJsonOptions());
			GridData = new GridData(PatternData.Rows, PatternData.Cols, CellSize);
			SetGridBackgroundTexture(GridData);
		}
	}

	public void Initialize()
	{
		Debug.Assert(GridData is not null);
		Debug.Assert(PatternData is not null);
		Game.GridData = GridData;	
		AddEntity(new Grid(GridData));
		DropTime = 750;
		TetrominoStack.Clear( );
		PatternData.Shapes.Reverse( );
		PatternData.Shapes.ForEach(s => TetrominoStack.Push(new Tetromino(s.Shape, Rotation.Up, ( GridData.Cols - Tetromino.BoundingBoxSize(s.Shape) ) / 2, GridData.CellSize)));
		AddEntity(new Pattern(PatternData, GridData));
		SpawnTetromino( );
	}

	public void OnGameEvent(IGameEvent e)
	{
		if (e is TetrominoLocked tl)
		{
			GetEntity<Grid>( ).LockCells(tl);
			GetEntity<Pattern>().CalculateScore(tl);

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