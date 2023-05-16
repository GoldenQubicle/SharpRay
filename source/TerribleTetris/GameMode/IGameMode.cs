namespace TerribleTetris.GameMode
{
    internal interface IGameMode
    {
        PatternData PatternData { get; set; }
        GridData GridData { get; set; }
        void Initialize();
        void OnGameEvent(IGameEvent e);

        IGameMode NextMode(IGameMode nextGameMode);
    }
}
