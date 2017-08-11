using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class ResourceManager
    {
        #region RessourceDefinition

        private Dictionary<VoxelMaterial, ResourceVeinData> _ressourceDefinitions = new Dictionary<VoxelMaterial, ResourceVeinData>()
        {
            {
                MaterialDefinition.All.Copper,
                new ResourceVeinData
                {
                    MinAmount = 5,
                    MaxAmount = 10,
                    MinVeinLength = 30,
                    Material = MaterialDefinition.All.Copper,
                    VeinRadius = 2
                }
            },
            {
                MaterialDefinition.All.Coal,
                new ResourceVeinData
                {
                    MinAmount = 5,
                    MaxAmount = 10,
                    MinVeinLength = 20,
                    Material = MaterialDefinition.All.Coal,
                    VeinRadius = 3
                }
            },

            {
                MaterialDefinition.All.Iron,
                new ResourceVeinData
                {
                    MinAmount = 3,
                    MaxAmount = 7,
                    MinVeinLength = 60,
                    Material = MaterialDefinition.All.Iron,
                    VeinRadius = 1
                }
            },
            {
                MaterialDefinition.All.Gold,
                new ResourceVeinData
                {
                    MinAmount = 1,
                    MaxAmount = 3,
                    MinVeinLength = 60,
                    Material = MaterialDefinition.All.Gold,
                    VeinRadius = 0.7f
                }
            }
        };
        #endregion

        public void SpawnAllResources(MapData map, Dictionary<VoxelMaterial, int> weight, float density = 1f)
        {
            var veinAmount = map.Chunks.GetLength(0)*map.Chunks.GetLength(0)*Chunk.ChunkSize*Chunk.ChunkSize/5000f*density;
            var multiplier = veinAmount/weight.Values.Sum();
            foreach (var material in weight.Keys)
            {
                for (var i = 0; i < multiplier*weight[material]; i++)
                {
                    SpawnResources(_ressourceDefinitions[material], map);
                }
            }
        }

        public void SpawnResources(ResourceVeinData data, MapData map)
        {
            var amount = Random.Range(data.MinAmount, data.MaxAmount+1);
            var width = map.Chunks.GetLength(0) * Chunk.ChunkSize;
            var height = map.Chunks.GetLength(0) * Chunk.ChunkSize;
            var start = Vector3.zero;
            do
            {
                start = new Vector3(Random.Range(0, width), Random.Range(0, height), Random.Range(0, width));

            } while (!IsStone(start, map));

            SpawnVein(data, map, start, amount);
        }

        private void SpawnVein(ResourceVeinData data, MapData map, Vector3 start, int veinsLeft)
        {
            veinsLeft--;
            var path = GetPath(data, map, start);
            DrawPath(data, path, map);
            if (veinsLeft > 0)
            {
                SpawnVein(data, map, path[Random.Range(0, 4) % path.Count], veinsLeft / 2);
                SpawnVein(data, map, path[Random.Range(0, 4) % path.Count], veinsLeft-(veinsLeft / 2));
            }

        }

        private void DrawPath(ResourceVeinData data, List<Vector3> path, MapData map)
        {
            for (var i = 0; i < path.Count - 2; i++)
            {
                //Debug.DrawLine(path[i], path[i+1], data.Material.Color, 6000, false);
                DrawCapsule(path[i], path[i + 1], data.VeinRadius, data.Material, map);
            }
        }

        private void DrawCapsule(Vector3 start, Vector3 end, float veinRadius, VoxelMaterial material, MapData map)
        {
            var minx = (int)(Mathf.Min(start.x, end.x) - veinRadius);
            var miny = (int)(Mathf.Min(start.y, end.y) - veinRadius);
            var minz = (int)(Mathf.Min(start.z, end.z) - veinRadius);
            var maxx = (int)(Mathf.Max(start.x, end.x) + veinRadius);
            var maxy = (int)(Mathf.Max(start.y, end.y) + veinRadius);
            var maxz = (int)(Mathf.Max(start.z, end.z) + veinRadius);
            var direction = (start - end).normalized;
            var ray = new Ray(start, direction);
            for (int x = minx; x < maxx; x++)
            {
                for (int y = miny; y < maxy; y++)
                {
                    for (int z = minz; z < maxz; z++)
                    {
                        if (Map.Instance.IsInBounds(x, y, z) && Vector3.Cross(ray.direction, new Vector3(x, y, z) - ray.origin).magnitude <= veinRadius)
                        {
                            map.SetVoxel(x, y, z, true, material);
                        }
                    }
                }
            }
        }

        private List<Vector3> GetPath(ResourceVeinData data, MapData map, Vector3 start)
        {
            var list = new List<Vector3>();
            list.Add(start);
            var length = 0f;
            do
            {
                var end = new Vector3(start.x + Random.Range(-data.MinVeinLength, data.MinVeinLength),
                                  start.y + Random.Range(-data.MinVeinLength / 3, data.MinVeinLength / 3),
                                  start.z + Random.Range(-data.MinVeinLength, data.MinVeinLength));
                if (IsStone(end, map) && (start-end).magnitude > data.MinVeinLength / 2f)
                {
                    list.Add(end);
                    length += (list[list.Count - 1] - list[list.Count - 2]).magnitude;
                }

            } while (length < data.MinVeinLength);
            return list;
        }

        private bool IsStone(Vector3 pos, MapData map)
        {
            return Map.Instance.IsInBounds((int)pos.x, (int)pos.y, (int)pos.z) &&
                map.Chunks[(int)(pos.x / Chunk.ChunkSize), (int)(pos.y / Chunk.ChunkSize), (int)(pos.z / Chunk.ChunkSize)] != null &&
                   map.Chunks[(int) (pos.x/Chunk.ChunkSize), (int) (pos.y/Chunk.ChunkSize), (int) (pos.z/Chunk.ChunkSize)]
                .GetVoxelType((int) (pos.x%Chunk.ChunkSize), (int) (pos.y%Chunk.ChunkSize),(int) (pos.z%Chunk.ChunkSize)).Equals(MaterialDefinition.All.Stone);
        }
    }

    public struct ResourceVeinData
    {
        public int MinAmount;
        public int MaxAmount;
        public int MinVeinLength;
        public float VeinRadius;
        public VoxelMaterial Material;
    }
}
