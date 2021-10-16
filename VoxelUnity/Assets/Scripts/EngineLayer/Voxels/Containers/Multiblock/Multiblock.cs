using System.Collections.Generic;
using System.Linq;
using AccessLayer.Material;
using EngineLayer.Voxels.Containers.Chunks;
using EngineLayer.Voxels.Data;
using EngineLayer.Voxels.Material;
using UnityEngine;

namespace EngineLayer.Voxels.Containers.Multiblock
{
    public class Multiblock : VoxelContainer
    {
        public static Multiblock InstantiateVoxels(Vector3 position, Dictionary<VoxelMaterial, List<Vector3>> voxels, string name)
        {
            var map = Map.Instance;
            var size = GetSize(out var zeroVec, voxels);
            var data = new ContainerData(size, position + zeroVec);
            foreach (var type in voxels.Keys)
            {
                foreach (var v in voxels[type])
                {
                    data.SetVoxel((int)(v.x - zeroVec.x), (int)(v.y - zeroVec.y), (int)(v.z - zeroVec.z), type);
                }
            }
            var container = CreateContainer<Multiblock>(position+zeroVec, data, MaterialRegistry.Instance.Materials, name);
            if (map != null)
            {
                container.transform.parent = map.transform;
                for (var x = Mathf.Max(0, (int)((position.x) / Chunk.ChunkSize)); x < Mathf.Min(map.MapData.Chunks.GetLength(0), (position.x + size) / Chunk.ChunkSize); x++)
                {
                    for (var y = Mathf.Max(0, (int)((position.y) / Chunk.ChunkSize)); y < Mathf.Min(map.MapData.Chunks.GetLength(1), (position.y + size) / Chunk.ChunkSize); y++)
                    {
                        for (var z = Mathf.Max(0, (int)((position.z) / Chunk.ChunkSize)); z < Mathf.Min(map.MapData.Chunks.GetLength(2), (position.z + size) / Chunk.ChunkSize); z++)
                        {
                            if (map.MapData.Chunks[x, y, z] != null)
                                map.MapData.Chunks[x, y, z].AttachMultiblock((Multiblock)container);
                        }
                    }
                }
            }
            return (Multiblock)container;
        }

        public void EnableWind(float intensity)
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            var min = mesh.vertices.Min(v => v.y);
            var max = mesh.vertices.Max(v => v.y);
            var height = max - min;
            var colors = new Color[mesh.vertexCount];
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                var r = (mesh.vertices[i].y + 0.5f) / height * intensity;
                colors[i] = new Color(r, 0, 0);
            }
            mesh.SetColors(colors.ToList());
        }

        private static int GetSize(out Vector3 zeroVec, Dictionary<VoxelMaterial, List<Vector3>> voxels)
        {
            var first = voxels.First().Value.First();
            float minX = first.x, minY = first.y, minZ = first.z, maxX = first.x, maxY = first.y, maxZ = first.z;
            foreach (var type in voxels.Keys)
            {
                foreach (var v in voxels[type])
                {
                    if (minX > v.x)
                        minX = v.x;
                    if (minY > v.y)
                        minY = v.y;
                    if (minZ > v.z)
                        minZ = v.z;
                    if (maxX < v.x)
                        maxX = v.x;
                    if (maxY < v.y)
                        maxY = v.y;
                    if (maxZ < v.z)
                        maxZ = v.z;
                }
            }
            var width = maxX - minX;
            var height = maxY - minY;
            var depth = maxZ - minZ;
            zeroVec = new Vector3(minX, minY, minZ);
            return (int)Mathf.Max(width, height, depth)+1;
        }
    }
}

