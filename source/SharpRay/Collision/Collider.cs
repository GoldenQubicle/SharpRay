using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay.Collision
{
    public abstract class Collider
    {
        protected static Color Color => Color.BLUE;
        public abstract void Render();

        public bool ContainsPoint(Vector2 point) => this switch
        {
            CircleCollider cc => CheckCollisionPointCircle(point, cc.Center, cc.Radius),
            RectCollider rc => CheckCollisionPointRec(point, rc.Collider),
            RectProCollider rpc => false,
            _ => false
        };

        public bool Overlaps(Collider collider) => (this, collider) switch
        {
            (CircleCollider cc, CircleCollider cco) => CheckCollisionCircles(cc.Center, cc.Radius, cco.Center, cco.Radius),
            (CircleCollider cc, RectCollider rc) => CheckCollisionCircleRec(cc.Center, cc.Radius, rc.Collider),
            (CircleCollider cc, RectProCollider rpc) => CheckCollisionRectProCircle(rpc, cc),
            (RectCollider rc, RectCollider rco) => CheckCollisionRecs(rc.Collider, rco.Collider),
            (RectCollider rc, CircleCollider cc) => CheckCollisionCircleRec(cc.Center, cc.Radius, rc.Collider),
            (RectCollider rc, RectProCollider rpc) => false,
            (RectProCollider rpc, RectProCollider rpco) => CheckCollisionRectProColliders(rpc, rpco),
            (RectProCollider rpc, CircleCollider cc) => CheckCollisionRectProCircle(rpc, cc),
            (RectProCollider rpc, RectCollider rc) => false,
            _ => false
        };

        private static bool CheckCollisionRectProColliders(RectProCollider rpc, RectProCollider rpco)
        {
            var cp = new Vector2();
            foreach (var line in rpc.GetLines())
                foreach (var otherLine in rpco.GetLines())
                    if (CheckCollisionLines(line.start, line.end, otherLine.start, otherLine.end, ref cp))
                        //Console.WriteLine($"rect pro collieder collision {rv}");
                        return true;
            return false;
        }

        private static bool CheckCollisionRectProCircle(RectProCollider rpc, CircleCollider cc)
        {
            foreach(var hp in cc.GetHitPoints())
                foreach (var (p1, p2, p3) in rpc.GetTriangles())
                    if (CheckCollisionPointTriangle(hp, p1, p2, p3))
                        return true;

            return false;
        }
    }
}
