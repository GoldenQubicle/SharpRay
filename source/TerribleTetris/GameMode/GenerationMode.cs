namespace TerribleTetris.GameMode;

internal class GenerationMode : IGameMode
{
	private readonly string _fileName;
	public PatternData PatternData { get; set; }
	public GridData GridData { get; set; }

	public GenerationMode(GridData gridData, string fileName = "")
	{
		_fileName = fileName;
		GridData = gridData;
	}

	public void Initialize()
	{
		SetGridBackgroundTexture(GridData);
        AddEntity(new Grid(GridData));
        DropTime = 10;
        TetrominoStack.Clear();
        PatternData = new PatternData(GridData.Rows, GridData.Cols, new());
        var shapes = Enum.GetValues<Shape>()[..7].ToList();
        var rotations = Enum.GetValues<Rotation>();
        var bagCount = 0;
        while (bagCount < 5)
        {
            var shape = shapes[GetRandomValue(0, shapes.Count - 1)];
            shapes.Remove(shape);
            TetrominoStack.Push(new Tetromino(shape, rotations[GetRandomValue(0, 3)], GetRandomValue(0, GridData.Cols - Tetromino.BoundingBoxSize(shape)), GridData.CellSize));

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
	            if (!string.IsNullOrEmpty(_fileName))
	            {
                    var json = JsonSerializer.Serialize(PatternData, GetJsonOptions( ));
                    File.WriteAllText(Path.Combine(AssestsFolder, _fileName), json);
                }
	            else
	            {
		            Game.GameMode = NextMode(new GenDoneMode( ));
				}

				return;
            }

            SpawnTetromino();
        }
    }

    public IGameMode NextMode(IGameMode nextGameMode)
    {
	    nextGameMode.PatternData = PatternData;
	    nextGameMode.GridData = GridData;
	    return nextGameMode;
    }
}