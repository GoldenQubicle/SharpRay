using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using Raylib_cs;
using System.Linq;
using static SharpRay.Core.Application;
using SharpRay.Gui;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace ProtoCity
{
    public class Zone : Entity
    {
        private readonly List<PointHandler> Points = new();
        private readonly List<Edge> Edges = new();

       public override void Render()
        {
            foreach (var (e, i) in Edges.Select((e, i) => (e, i)))
            {
                e.Render();
                DrawTextV(i.ToString(), e.C, 10, Color.BLACK);

                var next = Edges.FirstOrDefault(ne => e.B == ne.A);
                
                if (next == default) continue;

                var (_, start_e, end_e) = e.GetRays();
                var (_, start_ne, end_ne) = next.GetRays();

                var collision = new Vector2();

                CheckCollisionLines(start_e, end_e, start_ne, end_ne, ref collision);

                DrawCircleV(collision, 4, Color.ORANGE);
                DrawLineV(e.B.Position, collision, Color.ORANGE);
            }
        }
     
        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseRightClick mrc)
            {
                var ph = new PointHandler
                {
                    Position = mrc.Position,
                    Radius = 5,
                    ColorDefault = Color.BROWN,
                    ColorFocused = Color.BEIGE,
                };

                AddEntity(ph);
                Points.Add(ph);

                if (Points.Count >= 2)
                    Edges.Add(new Edge(Points[^2], Points[^1]));

                if (Edges.Count == 2)
                {
                    Edges.Add(new Edge(Points[^1], Points[0])); //close the zone but no need to remove an edge
                }
                else if (Edges.Count >= 2)
                {
                    Edges.Remove(Edges[^2]); //remove second to last edge since we've already added a new edge
                    Edges.Add(new Edge(Points[^1], Points[0])); // close the zone
                }
            }
        }
    }
}
