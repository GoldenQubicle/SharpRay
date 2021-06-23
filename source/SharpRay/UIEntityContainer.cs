using System.Numerics;
using System.Collections.Generic;
using System;

namespace SharpRay
{
    public static class UIEntityContainerBuilder
    {
        public static UIEntityContainer CreateNew(bool isVisible = true) => new UIEntityContainer(isVisible);

        public static UIEntityContainer AddChildren(this UIEntityContainer container, params UIEntity[] entities)
        {
            container.Entities.AddRange(entities);
            return container;
        }

        public static UIEntityContainer Translate(this UIEntityContainer container, Vector2 translate)
        {
            foreach (var e in container.Entities) e.Position += translate;
            return container;
        }

        public static UIEntityContainer SetOnUIEventAction(this UIEntityContainer container, Action<IUIEvent, Entity> onUIEventAction)
        {
            container.OnUIEventAction += onUIEventAction;
            return container;
        }
        public static UIEntityContainer SetOnGameEventAction(this UIEntityContainer container, Action<IGameEvent, Entity> onGameEventAction)
        {
            container.OnGameEventAction += onGameEventAction;
            return container;
        }
    }

    public class UIEntityContainer : Entity, IUIEventListener, IGameEventListener
    {
        public UIEntityContainer(bool isVisible = true) { IsVisible = isVisible; }
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

        public Action<IUIEvent, Entity> OnUIEventAction { get; set; }

        public void OnUIEvent(IUIEvent e) => OnUIEventAction?.Invoke(e, this);

        public Action<IGameEvent, Entity> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e) => OnGameEventAction?.Invoke(e, this);
    }
}

