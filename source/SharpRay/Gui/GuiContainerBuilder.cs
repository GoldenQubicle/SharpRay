using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;

namespace SharpRay.Gui
{
    public static class GuiContainerBuilder
    {
        public static GuiContainer CreateNew(bool isVisible = true) => new GuiContainer(isVisible);

        public static GuiContainer AddChildren(this GuiContainer container, params GuiEntity[] entities)
        {
            foreach(var e in entities)
                Application.SetEmitEventActions(e, Application.OnGuiEvent, Audio.OnGuiEvent, container.OnGuiEvent);

            container.Add(entities);
            return container;
        }

        public static GuiContainer Translate(this GuiContainer container, Vector2 translate)
        {
            container.TranslateEntities(translate);
            
            return container;
        }

        public static GuiContainer OnGuiEvent(this GuiContainer container, Action<IGuiEvent, GuiContainer> onUIEventAction)
        {
            container.OnGuiEventAction += onUIEventAction;
            return container;
        }

        public static GuiContainer OnGameEvent(this GuiContainer container, Action<IGameEvent, GuiContainer> onGameEventAction)
        {
            container.OnGameEventAction += onGameEventAction;
            return container;
        }
    }
}

