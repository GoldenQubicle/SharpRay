namespace TerribleTetris.GameMode;

internal class PauseMode : IGameMode
{
	public PatternData PatternData { get; set; }
	public void OnStart(GridData gridData)
	{
	}

	public void OnGameEvent(IGameEvent e)
	{
	}

	public IGameMode NextMode(IGameMode nextGameMode)
	{
		nextGameMode.PatternData = PatternData;
		return nextGameMode;
	}
}