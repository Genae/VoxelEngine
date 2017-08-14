using System.Collections;
using UnityEngine;
using Assets.Scripts.Data.Material;

namespace Assets.Scripts.Data.Map
{
    public class MapData
    {
        public ChunkData[,,] Chunks { get; private set; }
        public int Size, Height;
        public float Scale;
        
        public MapData(int size, int height, float scaleMultiplier)
        {
            Chunks = new ChunkData[(int)(size * scaleMultiplier), (int)(height * scaleMultiplier), (int)(size * scaleMultiplier)];
            Size = Chunks.GetLength(0);
            Height = Chunks.GetLength(1);
            Scale = scaleMultiplier;
        }

        public IEnumerator LoadHeightmap(float[,] heightmap, float[,] bottom, float[,] cut, float heightmapHeight)
        {
            for (var x = 0; x < Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (var z = 0; z < Chunks.GetLength(2) * Chunk.ChunkSize; z++)
                {
                    var bot = (short)((bottom[(int)(x /Scale), (int)(z /Scale)] + 2) / 3 * heightmapHeight);
                    var lheight = (heightmap[(int)(x /Scale), (int)(z /Scale)] + 2) / 3 * heightmapHeight;
                    for (var y = 0; y < Chunks.GetLength(1) * Chunk.ChunkSize; y++)
                    {
                        var isActive = y < (int) lheight && y > bot && cut[(int)(x /Scale), (int)(z /Scale)] > 0.5f;
                        if (!isActive)
                            continue;
                        var blockType = y == (int) lheight - 1 ? MaterialRegistry.Instance.GetMaterialFromName("Grass") : (y >= (int) lheight - 4 ? MaterialRegistry.Instance.GetMaterialFromName("Dirt") : MaterialRegistry.Instance.GetMaterialFromName("Stone"));
                        SetVoxel(x, y, z, true, blockType);
                    }
                }
                yield return null;
            }
            for (var x = 0; x < Chunks.GetLength(0); x++)
            {
                for (var y = 0; y < Chunks.GetLength(1); y++)
                {
                    for (var z = 0; z < Chunks.GetLength(2); z++)
                    {
                        SetIntoNeighbourContext(x, y, z);
                    }
                }
                yield return null;
            }
        }
        
        public VoxelData SetVoxel(int x, int y, int z, bool active, VoxelMaterial material, Multiblock.Multiblock mb = null)
        {
            var cx = x / Chunk.ChunkSize;
            var cy = y / Chunk.ChunkSize;
            var cz = z / Chunk.ChunkSize;
            if(Chunks[cx, cy, cz] == null)
                Chunks[cx, cy, cz] = new ChunkData(new Vector3(cx, cy, cz) * Chunk.ChunkSize);
            return Chunks[cx, cy, cz].SetVoxel(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize, active, material);
        }
        public VoxelMaterial GetVoxelMaterial(int x, int y, int z)
        {
            var chunk = Chunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
            return chunk.GetVoxelType(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize);
        }

        public VoxelMaterial GetVoxelMaterial(Vector3 pos)
        {
            return GetVoxelMaterial((int) pos.x, (int) pos.y, (int) pos.z);
        }

        private void SetIntoNeighbourContext(int x, int y, int z)
        {
            if (Chunks[x, y, z] == null)
                return;
            var borders = new bool[6][,];
            var solid = new[]
            {
                x == 0 || Chunks[x - 1, y, z] != null && !Chunks[x - 1, y, z].HasSolidBorder(1, out borders[0]),
                x == Chunks.GetLength(0) - 1 || Chunks[x + 1, y, z] != null && !Chunks[x + 1, y, z].HasSolidBorder(2, out borders[1]),
                y == 0 || Chunks[x, y - 1, z] != null && !Chunks[x, y - 1, z].HasSolidBorder(3, out borders[2]),
                y == Chunks.GetLength(1) - 1 || Chunks[x, y + 1, z] != null && !Chunks[x, y + 1, z].HasSolidBorder(4, out borders[3]),
                z == 0 || Chunks[x, y, z - 1] != null && !Chunks[x, y, z - 1].HasSolidBorder(5, out borders[4]),
                z == Chunks.GetLength(2) - 1 || Chunks[x, y, z + 1] != null && !Chunks[x, y, z + 1].HasSolidBorder(6, out borders[5])
            };
            var neighbourData = new ChunkData[6];
            if (x != 0) neighbourData[0] = Chunks[x - 1, y, z];
            if (x < Chunks.GetLength(0) - 1) neighbourData[1] = Chunks[x + 1, y, z];
            if (y != 0) neighbourData[2] = Chunks[x, y - 1, z];
            if (y < Chunks.GetLength(1) - 1) neighbourData[3] = Chunks[x, y + 1, z];
            if (z != 0) neighbourData[4] = Chunks[x, y, z - 1];
            if (z < Chunks.GetLength(2) - 1) neighbourData[5] = Chunks[x, y, z + 1];

            Chunks[x, y, z].UpdateBorder(borders, solid, neighbourData, false);
        }

    }
}
