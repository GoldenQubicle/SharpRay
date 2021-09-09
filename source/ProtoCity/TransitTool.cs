using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Gui;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class TransitTool : Entity
    {
        private bool isActive = true;

        public override void Render()
        {
            DrawText($"Transit Tool :  {(isActive ? "Active" : "InActive")}", 100, 10, 15, Color.RAYWHITE);
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (!isActive) return;

            if (e is MouseLeftClick mlc)
            {
                var o = GridHandler.GetCellOccupant(mlc.Position);

                if (o is Occupant.TransitNode)
                {
                    //so here how do we get the actual, existing node?
                }
                else if (o is Occupant.None)
                {
                    //player created a new transit segment
                    //with node A locked in place, 
                    //node B under cursor to drag out
                    var pos = GridHandler.IndexToCenterCoordinatesV(GridHandler.CoordinatesToIndex(mlc.Position));
                    AddEntity(new Edge(new PointHandler 
                    { 
                        Position = pos,
                        Radius = GridHandler.CellSize / 2,
                    }, new PointHandler 
                    {
                        Position = pos,
                        Radius = GridHandler.CellSize / 2,
                        IsSelected = true,
                    }));
                }
                else
                {
                    //player cannot draw anything from here
                }
            }


            if (e is MouseLeftDrag mld)
            {
                //need to check here if player is moving some node around to begin with
                //this will be either an end from a newly drawn stub OR
                //an existing node with N connections
            }

            if(e is MouseLeftRelease mlr)
            {
                //again need to check if player is actually move a node around to begin with
                //either an existing node has been moved
                //or node B of the new segment has been placed
                //either way, things need to be notified and updateded
            }
        }
    }

    
}
