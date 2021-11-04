using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;

namespace SharpRay.Gui
{
    public static class GuiEntityContainerBuilder
    {
        public static GuiEntityContainer CreateNew(bool isVisible = true) => new GuiEntityContainer(isVisible);

        public static GuiEntityContainer AddChildren(this GuiEntityContainer container, params GuiEntity[] entities)
        {
            foreach(var e in entities)
                Application.SetEmitEventActions(e, Application.OnGuiEvent, Audio.OnGuiEvent, container.OnGuiEvent);

            container.Entities.AddRange(entities);
            return container;
        }

        public static GuiEntityContainer Translate(this GuiEntityContainer container, Vector2 translate)
        {
            foreach (var e in container.Entities) e.Position += translate;
            return container;
        }

        public static GuiEntityContainer OnUIEvent(this GuiEntityContainer container, Action<IGuiEvent, GuiEntityContainer> onUIEventAction)
        {
            container.OnGuiEventAction += onUIEventAction;
            return container;
        }
        public static GuiEntityContainer OnGameEvent(this GuiEntityContainer container, Action<IGameEvent, GuiEntityContainer> onGameEventAction)
        {
            container.OnGameEventAction += onGameEventAction;
            return container;
        }
    }
}

