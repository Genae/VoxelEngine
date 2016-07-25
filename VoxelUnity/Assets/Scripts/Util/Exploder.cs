using UnityEngine;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;

namespace Assets.Scripts.Util
{
    public class Exploder : MonoBehaviour
    {

        public static void Explode(VoxelContainer voxelContainer)
        {

            var vc = voxelContainer;
            var cd = vc.ContainerData;

            for (var x = 1; x < cd.Size-1; x+=2)
            {
                for (var y = 1; y < cd.Size-1; y+=2)
                {
                    for (var z = 1; z < cd.Size-1; z+=2)
                    {
                        if (cd.Voxels[x, y, z] == null)
                            continue;
                        if (cd.Voxels[x, y, z].IsActive)
                        {
                            var pos = new Vector3(cd.Position.x + x, cd.Position.y + y, cd.Position.z + z);
                            var go = ObjectPool.Instance.GetObjectForType<Fraction>(parent:voxelContainer.transform.parent);
                            go.Init(pos, 2.0f, MaterialRegistry.MaterialFromId(cd.Voxels[x, y, z].BlockType).Color, voxelContainer.GetCenter());
                        }
                    }
                }
            }
            DestroyImmediate(voxelContainer.gameObject);
            
        }
    }
}
