﻿namespace AoC.Entities
{
    internal abstract class AoCEntity : Entity
    {
        protected readonly int AnimationSpeed = 1;

        protected ConcurrentDictionary<int, ConcurrentBag<Grid2d.Cell>> RenderUpdate = new();

        protected readonly ConcurrentDictionary<int, Color> RenderUpdateColor = new();


        public abstract Task RenderAction(IRenderState state, int layer = 0, Color color = default);

        protected AoCEntity(SharpRayConfig config, string part)
        {
            AddEntity(new Button
            {
                RenderLayer = 2,
                Text = "Start",
                DoCenterText = true,
                FontSize = 20,
                Size = new Vector2(100, 30),
                Position = new Vector2(config.WindowWidth - 75, config.WindowHeight - 40),
                TextColor = Color.RayWhite,
                FocusColor = Color.Green,
                BaseColor = Color.DarkGreen,
                OnMouseLeftClick = e => new AoCEvent(e, part),
                EmitEvent = GuiEvent
            });
        }
    }
}
