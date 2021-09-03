using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Linq;
using System;

namespace ProtoCity
{
    public class Zone : Entity
    {
        private readonly List<Vector2> Points = new();
        private readonly List<Edge> Edges = new();

        public override void Render()
        {
            Edges.ForEach(e => e.Render());

            foreach (var (p, i) in Points.Select((p, i) => (p, i)))
                DrawCircleV(p, 3, Color.DARKBROWN);
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseLeftClick mlc)
            {
                Points.Add(mlc.Position);

                if (Points.Count >= 2)
                    Edges.Add(new Edge(Points[Points.Count - 2], Points.Last()));

                if (Edges.Count == 2)
                {
                    Edges.Add(new Edge(Points.Last(), Points.First())); //close the zone but no need to remove an edge
                }
                else if (Edges.Count >= 2)
                {
                    Edges.Remove(Edges[Edges.Count - 2]); //remove second to last edge since we've already added a new edge
                    Edges.Add(new Edge(Points.Last(), Points.First())); // close the zone
                }
            }
        }
    }
}
