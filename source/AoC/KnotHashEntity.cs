using Common.Renders;
using Rectangle = Raylib_cs.Rectangle;

namespace AoC
{
	internal class KnotHashEntity(SharpRayConfig config) : AoCEntity(config)
	{
		private List<int> list = Enumerable.Range(0, 256).ToList();

		public override async Task RenderAction(IRenderState state, int layer = 0, Color color = default)
		{
			var update = state.Cast<KnotHashRender>();

			Console.WriteLine(update.Jump);

			await Task.Delay(AnimationSpeed);
		}

		public override void Render()
		{
			var chunkWidth = config.WindowWidth * .8f;
			var chunkHeight = ((config.WindowHeight * .45f) / 16f);
			var rectSize = chunkWidth / 16;
			var anchor = new Vector2(10, 5);
			var chunkSize = new Vector2(chunkWidth, rectSize);

			foreach (var chunk in list.Chunk(16).WithIndex())
			{
				var pos = anchor + new Vector2(0, (chunkHeight + 30) * chunk.idx);
				DrawRectangleLinesV(pos, chunkSize, Color.GREEN);
				//DrawTextV(chunk.idx.ToString(), p, 10, Color.BLACK );
				foreach (var c in chunk.Value.WithIndex())
				{
					var p = pos + new Vector2(rectSize * c.idx, 0) + new Vector2(rectSize * .4f, rectSize * .4f);
					DrawTextV(c.Value.ToString(), p, 10, Color.DARKPURPLE);
				}
			}

			DrawRectangleLinesEx(new Rectangle{ Height = rectSize, Width = rectSize, X = anchor.X, Y = anchor.Y }, 3, Color.RED);
		}

		
	}
}
