using Common.Renders;

namespace AoC
{
	internal class KnotHashEntity : AoCEntity
	{
		public KnotHashEntity(SharpRayConfig config) : base(config)
		{
		}

		public override async Task RenderAction(IRenderState state, int layer, Color color)
		{
			var update = (KnotHashRender)state;
			
			Console.WriteLine(update.Jump);

			await Task.Delay(_animationSpeed);
		}
	}
}
