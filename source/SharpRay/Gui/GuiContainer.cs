﻿using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SharpRay.Gui
{
    public sealed class GuiContainer : Entity, IGuiEventListener<GuiContainer>, IGameEventListener<GuiContainer>
    {
        public GuiContainer(bool isVisible = true, int renderLayer = 0, string tag = "")
        {
            IsVisible = isVisible;
            RenderLayer = renderLayer;
            Tag = tag;
        }

        private List<GuiEntity> Children { get; } = new();

        public bool IsVisible { get; private set; }

        public TEntity GetEntity<TEntity>() where TEntity : GuiEntity =>
            Children.OfType<TEntity>().FirstOrDefault();

        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : GuiEntity => 
            Children.OfType<TEntity>();
        public TEntity GetEntityByTag<TEntity>(string tag) where TEntity : Entity =>
            Children.OfType<TEntity>().FirstOrDefault(e => e.Tag.Equals(tag));

        public void Add(GuiEntity[] entities) => Children.AddRange(entities);

        public GuiEntity GetEntityByIndex(int idx) => Children[idx];

        public void Hide()
        {
            IsVisible = false;
            foreach (var e in Children) e.HasMouseFocus = false;
        }

        public void TranslateEntities(Vector2 translate)
        {
            foreach (var e in Children) e.Position += translate;
        }

        public void Show() => IsVisible = true;

        public override void Render()
        {
            if (IsVisible) foreach (var e in Children) e.Render();
        }

        public override void Update(double deltaTime)
        {
            if (IsVisible) foreach (var e in Children) e.Update(deltaTime);
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            if (IsVisible) foreach (var e in Children) e.OnMouseEvent(me);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent ke)
        {
            if (IsVisible) foreach (var e in Children) e.OnKeyBoardEvent(ke);
        }

        public Action<IGuiEvent, GuiContainer> OnGuiEventAction { get; set; }

        public void OnGuiEvent(IGuiEvent e) => OnGuiEventAction?.Invoke(e, this);

        public Action<IGameEvent, GuiContainer> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e) => OnGameEventAction?.Invoke(e, this);

    }
}

