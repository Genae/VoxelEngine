using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace VoxelEngine.Physics
{
    class SphereCollider : Collider
    {
        public float Range;

        public SphereCollider(Vector3 position, float range) : base(position, 0)
        {
            Range = range;
        }

    }
}
