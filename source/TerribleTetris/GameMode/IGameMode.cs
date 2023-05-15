namespace TerribleTetris.GameMode
{
    internal interface IGameMode
    {
        PatternData PatternData { get; set; }
        void OnStart(GridData gridData);
        void OnGameEvent(IGameEvent e);

        IGameMode NextMode(IGameMode nextGameMode);
    }
}
