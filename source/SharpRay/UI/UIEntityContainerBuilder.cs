using System.Numerics;
using System;

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
}

