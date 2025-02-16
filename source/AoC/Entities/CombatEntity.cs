using SharpRay.Components;

namespace AoC.Entities;

internal class CombatEntity : AoCEntity
{
	public List<UnitData> UnitData { get; }
	public static Vector2 CellSize = new(10, 10);
	private readonly IEnumerable<(Vector2, Color)> _gridRender;
	private readonly ConcurrentDictionary<int, CombatUnit> _units;
	private static readonly float UnitMoveSpeed = 150;
	private static readonly float UnitAttackSpeed = 75;

	public CombatEntity(Grid2d grid, List<UnitData> unitData, SharpRayConfig config, string part) : base(config, part)
	{

		CellSize = new(config.WindowWidth / grid.Width, config.WindowHeight / grid.Height);

		_gridRender = grid.Select(c => c.Character switch
		{
			'#' => (GridPosition2Screen(c.X, c.Y), Color.Brown),
			_ => (GridPosition2Screen(c.X, c.Y), Color.RayWhite),
		});

		_units = unitData
			.Select(u => new CombatUnit(u.Id, GridPosition2Screen(u.Position), u.Type == 'G' ? Color.DarkGreen : Color.SkyBlue))
			.ToConcurrentDictionary(u => u.Id, u => u);

		AddEntity(new AttackEntity());
	}

	public override async Task RenderAction(IRenderState state, int layer = 0, Color color = default)
	{
		if (state is not ICombatEvent)
			return; // 4 16 2024 dumb hack because of rendering is backed into aoc path finding...

		
		if (state is NewRound newRound)
		{

		}

		if (state is Move move)
		{
			_units[move.UnitId].DoMove(GridPosition2Screen(move.NewPosition));
			await Task.Delay((int)UnitMoveSpeed);
		}

		if (state is Attack attack)
		{
			AddEntity(new AttackEntity
			{
				Position = _units[attack.UnitId].Position,
				AttackPosition = _units[attack.TargetId].Position
			});

			await Task.Delay((int)UnitAttackSpeed);
		}

		if (state is Death death)
		{
			_units.Remove(death.UnitId, out _);
		}

		await Task.Delay(AnimationSpeed);
	}


	public override void Render()
	{
		_gridRender.ForEach(c =>
		{
			DrawRectangleV(c.Item1, CellSize, c.Item2);
		});

		_units.Values.ForEach(u => u.Render());
	}

	public override void Update(double deltaTime)
	{
		_units.Values.ForEach(u => u.Update(deltaTime));
	}

	private static Vector2 GridPosition2Screen((int x, int y) pos) => new(CellSize.X * pos.x, CellSize.Y * pos.y);
	private static Vector2 GridPosition2Screen(int x, int y) => new(CellSize.X * x, CellSize.Y * y);

	private class CombatUnit(int id, Vector2 position, Color color)
	{
		public int Id { get; } = id;
		public Color Color { get; set; } = color;

		public Vector2 Position { get; set; } = position;

		
		private bool _doMove;
		private readonly Easing _moveEasing = new (Easings.EaseSineIn, UnitMoveSpeed);
		private (Vector2 from, Vector2 to) _moveData;

		private bool _doAttack;
		private readonly Easing _attackEasing = new(Easings.EaseSineIn, UnitMoveSpeed);
		private Vector2 _attackPosition;

		public void Render()
		{
			DrawRectangleV(Position, CellSize, Color);

			//if (_doAttack)
			//{
			//	DrawLineV(Position + CellSize/2, _attackPosition + CellSize/ 2, Color.RED);
			//}
		}

		public void Update(double deltaTime)
		{
			if (_doMove)
			{
				_moveEasing.Update(deltaTime);
				Position = Vector2.Lerp(_moveData.from, _moveData.to, _moveEasing.GetValue( ));

				if (_moveEasing.IsDone())
					_doMove = false;
			}

			//if (_doAttack)
			//{
			//	_attackEasing.Update(deltaTime);

			//	if (_attackEasing.IsDone())
			//		_doAttack = false;
			//}
		}

		public void DoMove(Vector2 newPos)
		{
			_moveEasing.Reset( );
			_moveData = (Position, newPos);
			_doMove = true;
		}

		public void DoAttack(Vector2 targetPos)
		{
			_attackEasing.Reset();
			_attackPosition = targetPos;
			_doAttack = true;
		}
	}

	public class AttackEntity : Entity
	{
		private readonly Easing _attackEasing = new(Easings.EaseSineIn, UnitAttackSpeed);
		public Vector2 AttackPosition { get; set; }

		public AttackEntity()
		{
			RenderLayer = 1;
		}

		public override void Render()
		{
			var ap = AttackPosition + CellSize / 2;
			DrawLineV(Position + CellSize / 2, ap, Color.Red);
			DrawCircleV(Position + CellSize / 2, 5, Color.Red);
		}

		public override void Update(double deltaTime)
		{
			_attackEasing.Update(deltaTime);

			if (_attackEasing.IsDone( ))
				RemoveEntity(this);
		}
	}
}