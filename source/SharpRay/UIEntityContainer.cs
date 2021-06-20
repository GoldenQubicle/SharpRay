using System.Numerics;
using System.Collections.Generic;

namespace SharpRay
{
    public class UIEntityContainer : Entity, IUIEventListener
    {
        public List<UIEntity> Entities { get; }
        public Vector2 Translate { get; }
        public bool IsVisible { get; private set; } = true;

        public UIEntityContainer(List<UIEntity> entities, Vector2 translate)
        {
            Translate = translate;
            Entities = entities;

            foreach (var e in Entities) e.Position += Translate;
        }

        public void Hide()
        {
            IsVisible = false;
            foreach (var e in Entities) e.HasMouseFocus = false;
        }
        public void Show() => IsVisible = true;

        public override void Render( )
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
            if (ke is KeyPressed p && p.Char == 'M') Show();
            if (IsVisible) foreach (var e in Entities) e.OnKeyBoardEvent(ke);
        }

        public void OnUIEvent(IUIEvent e)
        {
            if (e is SnakeGameStart) Hide();
        }
    }
}

