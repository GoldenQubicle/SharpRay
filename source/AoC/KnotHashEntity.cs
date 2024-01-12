namespace AoC;

internal class KnotHashEntity(SharpRayConfig config) : AoCEntity(config)
{
	private const float MagicNumber = .8f;
	private readonly float _chunkHeight = ((config.WindowHeight * .45f) / 16f);
	private readonly float _rectSize = (config.WindowWidth * MagicNumber) / 16;
	private readonly Vector2 _anchor = new(10, 5);
	private readonly Vector2 _chunkSize = new(config.WindowWidth * MagicNumber, (config.WindowWidth * MagicNumber) / 16);
	private Vector2 _pointer = new(10, 5);
	private int _jump = 0;
	private List<int> list = Enumerable.Range(0, 256).ToList( );

	public override async Task RenderAction(IRenderState state, int layer = 0, Color color = default)
	{
		var update = state.Cast<KnotHashRender>( );

		_jump += update.Jump;
		_jump = _jump > 255 ? _jump % 255 : _jump;

		Console.WriteLine(_jump);
		
		await MovePointer(_anchor + ColRowToScreen(IdxToColRow(_jump)));


	}

	private bool _pointerIsMoving;
	private Vector2 _pointerNext;

	private async Task MovePointer(Vector2 to)
	{
		_pointerIsMoving = true;
		_pointerNext = to;

		while (_pointerIsMoving)
			await Task.Delay(5);

		await Task.Delay(750);
	}

	public override void Update(double deltaTime)
	{
		if (!_pointerIsMoving)
			return;

		_pointer.X += _rectSize;

		var max = ColRowToScreen(IdxToColRow(256)) - new Vector2(_rectSize, 0);

		if (_pointer.X >= max.X && _pointer.Y >= max.Y)
			_pointer = _anchor;

		if (_pointer.X >= _pointerNext.X - 5 && _pointer.Y == _pointerNext.Y)
		{
			_pointerIsMoving = false;
			_pointer = _pointerNext;
			return;
		}

		if (_pointer.X < _chunkSize.X - _rectSize) return;

		_pointer.X = _anchor.X;
		_pointer.Y += _chunkHeight + 30;

	}


	public override void Render()
	{
		foreach (var chunk in list.Chunk(16).WithIndex( ))
		{
			var pos = _anchor + new Vector2(0, (_chunkHeight + 30) * chunk.idx);
			DrawRectangleLinesV(pos, _chunkSize, Color.GREEN);
			//DrawTextV(chunk.idx.ToString(), p, 10, Color.BLACK );
			foreach (var c in chunk.Value.WithIndex( ))
			{
				var p = _anchor + ColRowToScreen(new(c.idx, chunk.idx)) + new Vector2(_rectSize * .4f, _rectSize * .4f);
				DrawTextV(c.Value.ToString( ), p, 10, Color.DARKPURPLE);
			}
		}

		DrawRectangleLinesEx(new Rectangle { Height = _rectSize, Width = _rectSize, X = _pointer.X, Y = _pointer.Y }, 3, Color.RED);

	}

	private Vector2 ColRowToScreen(Vector2 cr) => new(_rectSize * cr.X, (_chunkHeight + 30) * cr.Y);

	private Vector2 IdxToColRow(int idx) => new(idx % 16, idx / 16);


}