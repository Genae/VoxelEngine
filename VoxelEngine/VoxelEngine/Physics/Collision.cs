using System.Collections.Generic;

namespace VoxelEngine.Physics
{
    public class Collision
    {
        public static bool isColliding(Collider c1, Collider c2)
        {
            if ((c1.Position - c2.Position).Length < (c1.Range + c2.Range)) return true;
            return false;
        }

        public static List<List<Collider>> isColliding(List<Collider> list)
        {
            //TODO keine Zeit mehr

            return null;
        }
    }
}
