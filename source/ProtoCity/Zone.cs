using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using System.Collections.Generic;
using Raylib_cs;
using System.Linq;
using static SharpRay.Core.Application;
using SharpRay.Gui;

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
                //TODO fire off zone edit event to undo/redo adding point

                if (Points.Count >= 2)
                    Edges.Add(new Edge(Points[^2], Points[^1]));

                //TODO probably don't want to auto-close a zone
                if (Edges.Count == 2)
                {
                    Edges.Add(new Edge(Points[^1], Points[0])); //close the zone but no need to remove an edge
                }
                else if (Edges.Count >= 2)
                {
                    Edges.Remove(Edges[^2]); //remove second to last edge since we've already added a new edge
                    Edges.Add(new Edge(Points[^1], Points[0])); // close the zone
                }


                ph.OnDelete = p =>
                {
                    var edgesRemoved = Edges.Where(e => e.A == p || e.B == p).ToList();
                    Edges.Remove(edgesRemoved[0]);
                    Edges.Remove(edgesRemoved[1]);
                    RemoveEntity(p);
                    Points.Remove(p as PointHandler);
                    var edgeAdded = new Edge(edgesRemoved[0].A, edgesRemoved[1].B);
                    Edges.Add(edgeAdded);
                    return new ZoneDeletePoint
                    {
                        GuiComponent = p,
                    };
                };
            }

            

            ////delete a point poc
            ////when triggering it from handler we need to know which 
            //if (e is MouseLeftClick mlc)
            //{
            //    if (Points.Count <= 2) return;

            //    var lp = Points[^1];
            //    Edges.RemoveAll(e => e.A == lp || e.B == lp);
            //    Points.Remove(lp);
            //    RemoveEntity(lp);
                
            //    if(Edges.Count >=2)
            //        Edges.Add(new Edge(Points[^1], Points[0])); // close the zone
            //}
        }
    }

    public struct ZoneDeletePoint : IGuiEvent, IHasUndoRedo
    {
        public GuiEntity GuiComponent { get; init; }

        public void Undo() => throw new System.NotImplementedException();
        public void Redo() => throw new System.NotImplementedException();

    }
}
