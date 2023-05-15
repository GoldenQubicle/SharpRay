namespace TerribleTetris.GameMode;

internal class GenerationMode : IGameMode
{
	public PatternData PatternData { get; set; }

	public void OnStart(GridData gridData)
    {
        AddEntity(new Grid(gridData));
        DropTime = 10;
        TetrominoStack.Clear();
        PatternData = new PatternData(gridData.Rows, gridData.Cols, new());
        var shapes = Enum.GetValues<Shape>()[..7].ToList();
        var rotations = Enum.GetValues<Rotation>();
        var bagCount = 0;
        while (bagCount < 5)
        {
            var shape = shapes[GetRandomValue(0, shapes.Count - 1)];
            shapes.Remove(shape);
            TetrominoStack.Push(new Tetromino(shape, rotations[GetRandomValue(0, 3)], GetRandomValue(0, gridData.Cols - Tetromino.BoundingBoxSize(shape)), gridData.CellSize));

            if (shapes.Count != 0)
                continue;

            shapes = Enum.GetValues<Shape>()[..7].ToList();
            bagCount++;
        }

        SpawnTetromino();
    }

    public void OnGameEvent(IGameEvent e)
    {
        if (e is TetrominoLocked tl)
        {
            GetEntity<Grid>().LockCells(tl);
            PatternData.Shapes.Add(tl);

            if (IsAboveGrid(tl))
            {

	           Game.GameMode = NextMode(new PauseMode());
                return;
            }

            SpawnTetromino();
        }
    }

    public IGameMode NextMode(IGameMode nextGameMode)
    {
	    nextGameMode.PatternData = PatternData;
	    return nextGameMode;
    }
}