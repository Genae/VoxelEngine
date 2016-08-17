using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace VoxelEngine.Physics
{
    class BoxCollider : Collider
    {
        public float X, Y, Z;
        public BoxCollider(Vector3 position, float width, float height, float depth) : base(position, 1)
        {
            X = width;
            Y = height;
            Z = depth;
        }
    }
}
