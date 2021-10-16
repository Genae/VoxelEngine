using EngineLayer.Voxels.Containers;
using EngineLayer.Voxels.Containers.Chunks;
using UnityEngine;

namespace AccessLayer
{
    public class UnitManager
    {
        public static GameObject SpawnUnitAtRandomPosition(GameObject preset)
        {
            while (true)
            {
                var pos = new Vector3(Random.Range(0, Map.Instance.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.Instance.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                Physics.Raycast(new Ray(pos, Vector3.down), out var hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    var start = hit.point;
                    var obj = Object.Instantiate(preset, start, Quaternion.identity);
                    return obj;
                }
            }
        }
    }
}
