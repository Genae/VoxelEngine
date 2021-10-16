using System.Collections.Generic;
using System.Linq;
using AccessLayer.Material;
using AccessLayer.Worlds;
using EngineLayer.Voxels.Containers;
using EngineLayer.Voxels.Containers.Chunks;
using EngineLayer.Voxels.Data;
using EngineLayer.Voxels.Material;
using UnityEngine;

namespace AccessLayer
{
    public class ResourceManager
    {
        public void SpawnAllResources(MapData map, ResourceConfiguration[] resources, float density = 1f)
        {
            var veinAmount = map.Chunks.GetLength(0)*map.Chunks.GetLength(0)*Chunk.ChunkSize*Chunk.ChunkSize/5000f*density;
            var multiplier = veinAmount/ resources.Sum(r => r.Frequency);
            foreach (var material in resources)
            {
                for (var i = 0; i < multiplier* material.Frequency; i++)
                {
                    SpawnResources(material, map);
                }
            }
        }

        public void SpawnResources(ResourceConfiguration data, MapData map)
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

        private void SpawnVein(ResourceConfiguration data, MapData map, Vector3 start, int veinsLeft)
        {
            veinsLeft--;
            var path = GetPath(data, map, start);
            DrawPath(data, path);
            if (veinsLeft > 0)
            {
                SpawnVein(data, map, path[Random.Range(0, 4) % path.Count], veinsLeft / 2);
                SpawnVein(data, map, path[Random.Range(0, 4) % path.Count], veinsLeft-(veinsLeft / 2));
            }

        }

        private void DrawPath(ResourceConfiguration data, List<Vector3> path)
        {
            for (var i = 0; i < path.Count - 2; i++)
            {
                //Debug.DrawLine(path[i], path[i+1], data.Material.Color, 6000, false);
                DrawCapsule(path[i], path[i + 1], data.VeinRadius, data.Material);
            }
        }

        public static void DrawCapsule(Vector3 start, Vector3 end, float veinRadius, VoxelMaterial material, VoxelMaterial replace = null)
        {
            var minx = (int)(Mathf.Min(start.x, end.x) - veinRadius);
            var miny = (int)(Mathf.Min(start.y, end.y) - veinRadius);
            var minz = (int)(Mathf.Min(start.z, end.z) - veinRadius);
            var maxx = (int)(Mathf.Max(start.x, end.x) + veinRadius);
            var maxy = (int)(Mathf.Max(start.y, end.y) + veinRadius);
            var maxz = (int)(Mathf.Max(start.z, end.z) + veinRadius);
            var direction = (start - end).normalized;
            var ray = new Ray(start, direction);
            for (var x = minx; x < maxx; x++)
            {
                for (var y = miny; y < maxy; y++)
                {
                    for (var z = minz; z < maxz; z++)
                    {
                        if (!Map.Instance.IsInBounds(x, y, z))
                            continue;
                        if (Vector3.Cross(ray.direction, new Vector3(x, y, z) - ray.origin).magnitude <= veinRadius)
                        {
                            var intersect = ray.origin + ray.direction * Vector3.Dot(ray.direction, new Vector3(x, y, z) - ray.origin);
                            if(!((start - end).magnitude < (start - intersect).magnitude || (end - start).magnitude < (end - intersect).magnitude))
                                if(replace == null || World.At(x, y, z).GetMaterial().Equals(replace))
                                    World.At(x, y, z).SetVoxel(material);
                        }
                        if ((new Vector3(x, y, z) - start).magnitude <= veinRadius || (new Vector3(x, y, z) - end).magnitude <= veinRadius)
                        {
                            if (replace == null || World.At(x, y, z).GetMaterial().Equals(replace))
                                World.At(x, y, z).SetVoxel(material);
                        }
                    }
                }
            }
        }

        private List<Vector3> GetPath(ResourceConfiguration data, MapData map, Vector3 start)
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
                .GetVoxelType((int) (pos.x%Chunk.ChunkSize), (int) (pos.y%Chunk.ChunkSize),(int) (pos.z%Chunk.ChunkSize)).Equals(MaterialRegistry.Instance.GetMaterialFromName("Stone"));
        }
    }
}
