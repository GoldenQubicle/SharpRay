namespace TerribleTetris;

internal static partial class Game
{
	public record TetrominoBlocked(Shape Shape, List<(int, int)> Indices) : IGameEvent;


	internal class Tetromino : Entity, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>
	{
		public Action<IGameEvent> EmitEvent { get; set; }

		public (int x, int y) GetLeftMostX() => _data.Offsets[Rotation].MinBy(o => o.x);

		public (int x, int y) GetRightMostX() => _data.Offsets[Rotation].MaxBy(o => o.x);

		public List<(int x, int y)> GetRotationOffsets(Rotation rotation) =>
			_data.Offsets[rotation];

		private readonly TetrominoData _data;
		private readonly Easing _easing;
		private readonly Vector2 _bbSize;
		public Rotation Rotation { get; set; }
		public float X { get; set; }
		public float Y => _mapY.start;
		
		private (float start, float end) _mapY;
		private readonly List<(Vector2, int, int)> _debugIndices = new( );
		private bool _isActive = true;

		public Tetromino(Shape shape, int startCol)
		{
			Position = GridData.Position + new Vector2(startCol * GridData.CellSize, 0);
			_data = new TetrominoData(shape);
			_bbSize = new(_data.BoundingBoxSize * GridData.CellSize, _data.BoundingBoxSize * GridData.CellSize);
			_easing = new Easing(Easings.EaseExpoInOut, LevelTimer, isRepeated: true);
			_mapY = (Position.Y, Position.Y + GridData.CellSize);
			X = Position.X;
			CanMoveDown( );

		}

		public override void Render()
		{
			DrawRectangleLinesV(Position, _bbSize, BLUE);

			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = Position + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				DrawRectangleV(pos, new Vector2(GridData.CellSize, GridData.CellSize), _data.Color);
				//DrawCircleV(pos, 3, YELLOW);
			}

			_debugIndices.ForEach(t => DrawTextV($"{t.Item2}, {t.Item3}", t.Item1, 8, YELLOW));
		}

		public override void Update(double deltaTime)
		{
			if (!_isActive) return;

			if (!CanMoveDown() && _isActive)
			{
				EmitEvent(new TetrominoBlocked(_data.Shape, _data.Offsets[Rotation]
					.Select(o => TetrominoOffsetToGridIndices(o, Position)).ToList( )));
				_isActive = false;
				return;
			}

			_easing.Update(deltaTime);

			if (_easing.IsDone( ) && CanMoveDown( ))
			{
				_mapY = (Position.Y, Position.Y + GridData.CellSize);
			}
	
			Position = new Vector2(X, MapRange(_easing.GetValue( ), 0f, 1f, _mapY.start, _mapY.end));
		}


		private bool CanMoveDown()
		{
			_debugIndices.Clear( );
			var anchor = new Vector2(X, _mapY.end);
			foreach (var offset in _data.Offsets[Rotation])
			{
				var pos = Position + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
				var (x, y) = TetrominoOffsetToGridIndices(offset, anchor);
				_debugIndices.Add((pos, x, y));
			}

			var b = Grid.CanMove(_data.Offsets[Rotation], anchor + new Vector2(0, GridData.CellSize));

			return b;
		}
	}
}