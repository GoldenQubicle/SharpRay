namespace AoC.Entities;

internal class KnotHashEntity(SharpRayConfig config, string part) : AoCEntity(config, part)
{
    private const float MagicNumber = .8f;
    private readonly float _chunkHeight = config.WindowHeight * .45f / 16f;
    private readonly float _rectSize = config.WindowWidth * MagicNumber / 16;
    private readonly Vector2 _anchor = new(10, 5);
    private readonly Vector2 _chunkSize = new(config.WindowWidth * MagicNumber, config.WindowWidth * MagicNumber / 16);

    private readonly ConcurrentBag<int> _rangeRects = new();
    private readonly ConcurrentDictionary<int, int> _list =
        Enumerable.Range(0, 256).ToConcurrentDictionary(n => n, n => n);

    private int _idx;
    private int _idxNext;
    private Vector2 _pointer = new(10, 5);
    private bool _pointerIsMoving;
    private readonly bool _showPointer = false;
    private const int PauseTime = 500;

    private int _cycle;
    private int _op;

    public override async Task RenderAction(IRenderState state, int layer = 0, Color color = default)
    {
	    var update = state.Cast<KnotHashRender>();
        _cycle = update.Cycle;
        _op = update.Operation;

        await UpdateRange(update.Range);

        _idxNext = ClampIdx(_idx + update.Jump);

        if (_showPointer)
            await MovePointer();

        _idx = _idxNext;
    }



    private async Task UpdateRange(List<int> range)
    {
        foreach (var t in range.WithIndex())
        {
            _rangeRects.Add(t.Value);
            //await Task.Delay(AnimationSpeed);
        }

        foreach (var t in range.WithIndex())
        {
            _list[ClampIdx(_idx + t.idx)] = t.Value;
            await Task.Delay(AnimationSpeed);
        }

        await Task.Delay(AnimationSpeed);
        _rangeRects.Clear();
    }

    private async Task MovePointer()
    {
        _pointerIsMoving = true;

        while (_pointerIsMoving)
            await Task.Delay(AnimationSpeed);


        await Task.Delay(PauseTime);
    }

    public override void Update(double deltaTime)
    {
        DoMovePointer();
    }



    private void DoMovePointer()
    {
        if (!_pointerIsMoving)
            return;

        _pointer.X += _rectSize * 1f;

        if (ScreenToIdx(_pointer) == _idxNext + 1)
        {
            _pointerIsMoving = false;
            return;
        }


        if (_pointer.X > _chunkSize.X)
        {
            _pointer.X = _anchor.X;
            _pointer.Y += _chunkHeight + 30;
        }

        var max = ColRowToScreen(IdxToColRow(256)) - new Vector2(_rectSize, 0);

        if (_pointer.X >= max.X && _pointer.Y >= max.Y)
        {
            _pointer = _anchor;
        }
    }


    public override void Render()
    {
        foreach (var chunk in _list.Values.Chunk(16).WithIndex())
        {
            var pos = _anchor + new Vector2(0, (_chunkHeight + 30) * chunk.idx);
            DrawRectangleLinesV(pos, _chunkSize, Color.Green);

            var hash = chunk.Value.Skip(1).Aggregate(chunk.Value[0], (s, t) => s ^ t);
            DrawTextV(hash.ToString(), pos + new Vector2(_chunkSize.X + 50, _rectSize * .4f), 16, Color.RayWhite);


            foreach (var c in chunk.Value.WithIndex())
            {
                var p = _anchor + ColRowToScreen(new(c.idx, chunk.idx)) + new Vector2(_rectSize * .25f, _rectSize * .4f);
                DrawTextV(c.Value.ToString(), p, 16, Color.Gold);
            }
        }

        if (_showPointer)
            DrawRectangleLinesEx(new Rectangle { Height = _rectSize, Width = _rectSize, X = _pointer.X, Y = _pointer.Y }, 3, Color.Orange);

        foreach (var t in _rangeRects.WithIndex())
        {
            var p = _anchor + ColRowToScreen(IdxToColRow(ClampIdx(_idx + t.idx)));
            DrawRectangleV(p, new Vector2(_rectSize, _rectSize), ColorAlpha(Color.Purple, .25f));
        }

        DrawTextV($"Cycle {_cycle} / 64", _anchor with { Y = config.WindowHeight - 70 }, 16, Color.RayWhite);
        DrawTextV($"Operation {_op} / 58", _anchor with { Y = config.WindowHeight - 40 }, 16, Color.RayWhite);
    }

    private int ScreenToIdx(Vector2 pos) => (int)Math.Floor(pos.X / _rectSize + pos.Y / (_chunkHeight + 30) * 16);

    private Vector2 ColRowToScreen(Vector2 cr) => new(_rectSize * cr.X, (_chunkHeight + 30) * cr.Y);

    private static Vector2 IdxToColRow(int idx) => new(idx % 16, idx / 16 > 16 ? idx / 16 - 17 : idx / 16);

    private static int ClampIdx(int idx) => idx > 255 ? idx % 256 : idx;
}