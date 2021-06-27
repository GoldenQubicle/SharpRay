using System.Collections.Generic;
using System;

namespace SharpRay
{
    public sealed class UIEntityContainer : Entity, IUIEventListener<UIEntityContainer>, IGameEventListener<UIEntityContainer>
    {
        public UIEntityContainer(bool isVisible = true) => IsVisible = isVisible;
        public List<UIEntity> Entities { get; } = new();
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

        public Action<IUIEvent, UIEntityContainer> OnUIEventAction { get; set; }

        public void OnUIEvent(IUIEvent e) => OnUIEventAction?.Invoke(e, this);

        public Action<IGameEvent, UIEntityContainer> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e) => OnGameEventAction?.Invoke(e, this);
    }
}

