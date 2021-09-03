using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Linq;

namespace ProtoCity
{
    public class Zone : Entity
    {
        private readonly List<Vector2> Points = new();

        public override void Render()
        {
            foreach (var (p, i) in Points.Select((p, i) => (p, i)))
            {
                DrawCircleV(p, 3, Color.BEIGE);

                if (i >= 1)
                    DrawLineV(Points[i - 1], p, Color.BEIGE);

                if (i == Points.Count - 1)
                    DrawLineV(p, Points[0], Color.BEIGE);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseLeftClick mlc)
            {
                Points.Add(mlc.Position);
            }
        }
    }
}
