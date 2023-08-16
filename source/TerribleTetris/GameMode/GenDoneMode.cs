namespace TerribleTetris.GameMode;

internal class LevelDoneMode : IGameMode
{
	public PatternData PatternData { get; set; }
	public GridData GridData { get; set; }
	public void Initialize()
	{
		
	}

	public void OnGameEvent(IGameEvent e)
	{
	}

	public IGameMode NextMode(IGameMode nextGameMode)
	{
		return nextGameMode;
	}
}

internal class GenDoneMode : IGameMode
{
	public PatternData PatternData { get; set; }
	public GridData GridData { get; set; }

	public void Initialize()
	{
	}

	public void OnGameEvent(IGameEvent e)
	{
	}

	public IGameMode NextMode(IGameMode nextGameMode)
	{
		nextGameMode.PatternData = PatternData;
		nextGameMode.GridData = GridData;
		return nextGameMode;
	}
}