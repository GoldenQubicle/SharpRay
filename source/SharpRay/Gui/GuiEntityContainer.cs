using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Listeners;
using System;
using System.Collections.Generic;

namespace SharpRay.Gui
{
    public sealed class GuiEntityContainer : Entity, IGuiEventListener<GuiEntityContainer>, IGameEventListener<GuiEntityContainer>
    {
        public GuiEntityContainer(bool isVisible = true) => IsVisible = isVisible;
        public List<GuiEntity> Entities { get; } = new();
        public bool IsVisible { get; private set; }

        public void Hide()
        {
            IsVisible = false;
            foreach (var e in Entities) e.HasMouseFocus = false;
        }
        public void Show() => IsVisible = true;

        public override void Render()
        {
            if (IsVisible) foreach (var e in Entities) e.Render();
        }

        public override void Update(double deltaTime)
        {
            if (IsVisible) foreach (var e in Entities) e.Update(deltaTime);
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            if (IsVisible) foreach (var e in Entities) e.OnMouseEvent(me);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (IsVisible) foreach (var e in Entities) e.OnKeyBoardEvent(ke);
        }

        public Action<IGuiEvent, GuiEntityContainer> OnGuiEventAction { get; set; }

        public void OnGuiEvent(IGuiEvent e) => OnGuiEventAction?.Invoke(e, this);

        public Action<IGameEvent, GuiEntityContainer> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e) => OnGameEventAction?.Invoke(e, this);
    }
}

