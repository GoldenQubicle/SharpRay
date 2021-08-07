using SharpRay.Entities;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Numerics;
using SharpRay.Core;
using SharpRay.Collision;
using Raylib_cs;

namespace Asteroids
{
    public class World : GameEntity
    {
        public Vector2 Origin { get; init; }

        private float Rotation { get; set; }

        public override void Render()
        {
            //System.Console.WriteLine(Position);
            DrawRectanglePro((Collider as RectCollider).Collider, Origin, Rotation, DARKGREEN);

            //DrawRectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, DARKPURPLE);
            DrawRectangleRec((Collider as RectCollider).Collider, YELLOW);

            
        }
        public override void Update(double deltaTime)
        {
            (Collider as RectCollider).Position = Position;

    
        }

        private bool IsDragged { get; set; }
        private Vector2 DragOffSet { get; set; }
        private bool HasMouseFocus { get; set; }
        public Camera2D camera { get; private set; }

        public override void OnMouseEvent(IMouseEvent me)
        {
            HasMouseFocus = Collider.ContainsPoint(me.Position);

            if (me is MouseLeftDrag && IsDragged)
                Position = me.Position + DragOffSet;

            if(me is MouseRightDrag && IsDragged)
            {
                Rotation = (float) Application.MapRange(me.Position.Y, 0, Game.WindowHeight, 0, 180);
            }

            if (!HasMouseFocus) return;

            if (me is MouseLeftDrag && !IsDragged)
            {
                DragOffSet = Position - me.Position;
                IsDragged = true;
            }

            if (me is MouseRightDrag && !IsDragged)
            {
                IsDragged = true;
            }

            if ((me is MouseLeftRelease || me is MouseRightRelease) && IsDragged)
            {
                IsDragged = false;
            }
        }

    }
}
