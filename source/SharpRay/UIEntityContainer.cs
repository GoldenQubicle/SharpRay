using System.Numerics;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SharpRay
{
    public static class UIEntityContainerBuilder
    {
        public static UIEntityContainer CreateNew(bool isVisible = true) => new UIEntityContainer(isVisible);

        public static UIEntityContainer AddChildren(this UIEntityContainer container, params UIEntity[] entities)
        {
            foreach(var e in entities)
                Program.SetEmitEventActions(e, Program.OnUIEvent, Audio.OnUIEvent, container.OnUIEvent);

            container.Entities.AddRange(entities);
            return container;
        }

        public static UIEntityContainer Translate(this UIEntityContainer container, Vector2 translate)
        {
            foreach (var e in container.Entities) e.Position += translate;
            return container;
        }

        public static UIEntityContainer OnUIEvent(this UIEntityContainer container, Action<IUIEvent, UIEntityContainer> onUIEventAction)
        {
            container.OnUIEventAction += onUIEventAction;
            return container;
        }
        public static UIEntityContainer OnGameEvent(this UIEntityContainer container, Action<IGameEvent, UIEntityContainer> onGameEventAction)
        {
            container.OnGameEventAction += onGameEventAction;
            return container;
        }
    }

    public class UIEntityContainer : Entity, IUIEventListener<UIEntityContainer>, IGameEventListener<UIEntityContainer>
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

