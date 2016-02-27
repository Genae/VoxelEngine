using System;

namespace Assets.Scripts.Base.Data.Map
{
    public class MapData
    {
        public ChunkData[,,] Chunks { get; private set; }

        protected MapData(int size, int height)
        {
            Chunks = new ChunkData[size, height, size];
        }

        public static MapData CreateEmpty(int size, int height)
        {
            var map = new MapData(size / ChunkData.ChunkSize, height / ChunkData.ChunkSize);
            for (int x = 0; x < size / ChunkData.ChunkSize; x++)
            {
                for (int y = 0; y < height / ChunkData.ChunkSize; y++)
                {
                    for (int z = 0; z < size / ChunkData.ChunkSize; z++)
                    {
                        map.Chunks[x, y, z] = new ChunkData();
                    }
                }
            }
            for (int x = 0; x < map.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < map.Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < map.Chunks.GetLength(2); z++)
                    {
                        map.SetIntoNeighbourContext(x, y, z);
                    }
                }
            }
            return map;
        }

        public static MapData LoadHeightmap(short[,] heightmap, int height)
        {
            var size = heightmap.GetLength(0);
            var map = new MapData(size / ChunkData.ChunkSize, height / ChunkData.ChunkSize);
            for (int x = 0; x < size / ChunkData.ChunkSize; x++)
            {
                for (int y = 0; y < height / ChunkData.ChunkSize; y++)
                {
                    for (int z = 0; z < size / ChunkData.ChunkSize; z++)
                    {
                        map.Chunks[x, y, z] = new ChunkData();
                    }
                }
            }
            for (int x = 0; x < map.Chunks.GetLength(0) * ChunkData.ChunkSize; x++)
            {
                for (int z = 0; z < map.Chunks.GetLength(2) * ChunkData.ChunkSize; z++)
                {
                    var lheight = heightmap[x, z];
                    for (int y = 0; y < lheight; y++)
                    {
                        map.GetVoxel(x, y, z).IsActive = true;
                    }
                }
            }
            for (int x = 0; x < map.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < map.Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < map.Chunks.GetLength(2); z++)
                    {
                        map.SetIntoNeighbourContext(x, y, z);
                    }
                }
            }
            return map;
        }

        public static MapData LoadHeightmap(float[,] heightmap, int height)
        {
            var v = 0;
            var size = heightmap.GetLength(0);
            var map = new MapData(size / ChunkData.ChunkSize, height / ChunkData.ChunkSize);
            for (int x = 0; x < size / ChunkData.ChunkSize; x++)
            {
                for (int y = 0; y < height / ChunkData.ChunkSize; y++)
                {
                    for (int z = 0; z < size / ChunkData.ChunkSize; z++)
                    {
                        map.Chunks[x, y, z] = new ChunkData();
                    }
                }
            }
            for (int x = 0; x < map.Chunks.GetLength(0) * ChunkData.ChunkSize; x++)
            {
                for (int z = 0; z < map.Chunks.GetLength(2) * ChunkData.ChunkSize; z++)
                {
                    var lheight = heightmap[x, z] * height;
                    for (int y = 0; y < lheight; y++)
                    {
                        map.GetVoxel(x, y, z).IsActive = true;
                        v++;
                    }
                }
            }
            Console.WriteLine(v);
            for (int x = 0; x < map.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < map.Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < map.Chunks.GetLength(2); z++)
                    {
                        map.SetIntoNeighbourContext(x, y, z);
                    }
                }
            }
            return map;
        }

        public static MapData LoadHeightmap(float[,] heightmap, float[,] bottom, float[,] cut, short height, float heightmapHeight)
        {
            var v = 0;
            var size = heightmap.GetLength(0);
            var map = new MapData(size / ChunkData.ChunkSize, height / ChunkData.ChunkSize);
            for (int x = 0; x < size / ChunkData.ChunkSize; x++)
            {
                for (int y = 0; y < height / ChunkData.ChunkSize; y++)
                {
                    for (int z = 0; z < size / ChunkData.ChunkSize; z++)
                    {
                        map.Chunks[x, y, z] = new ChunkData();
                    }
                }
            }
            for (int x = 0; x < map.Chunks.GetLength(0) * ChunkData.ChunkSize; x++)
            {
                for (int z = 0; z < map.Chunks.GetLength(2) * ChunkData.ChunkSize; z++)
                {
                    if (cut[x, z] <= 0.5f)
                        continue;
                    var bot = (short)((bottom[x, z] + 2) / 3 * heightmapHeight);
                    var lheight = (heightmap[x, z] + 2) / 3 * heightmapHeight;
                    for (int y = bot; y < lheight; y++)
                    {
                        map.GetVoxel(x, y, z).IsActive = true;
                        v++;
                    }
                }
            }
            Console.WriteLine(v);
            for (int x = 0; x < map.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < map.Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < map.Chunks.GetLength(2); z++)
                    {
                        map.SetIntoNeighbourContext(x, y, z);
                    }
                }
            }
            return map;
        }

        public VoxelData GetVoxel(int x, int y, int z)
        {
            var cx = x / ChunkData.ChunkSize;
            var cy = y / ChunkData.ChunkSize;
            var cz = z / ChunkData.ChunkSize;
            return Chunks[cx, cy, cz].Voxels[x % ChunkData.ChunkSize, y % ChunkData.ChunkSize, z % ChunkData.ChunkSize];
        }

        private void SetIntoNeighbourContext(int x, int y, int z)
        {
            var borders = new bool[6][,];
            var solid = new[]
            {
                x == 0 || !Chunks[x - 1, y, z].HasSolidBorder(1, out borders[0]),
                x == Chunks.GetLength(0) - 1 || !Chunks[x + 1, y, z].HasSolidBorder(2, out borders[1]),
                y == 0 || !Chunks[x, y - 1, z].HasSolidBorder(3, out borders[2]),
                y == Chunks.GetLength(1) - 1 || !Chunks[x, y + 1, z].HasSolidBorder(4, out borders[3]),
                z == 0 || !Chunks[x, y, z - 1].HasSolidBorder(5, out borders[4]),
                z == Chunks.GetLength(2) - 1 || !Chunks[x, y, z + 1].HasSolidBorder(6, out borders[5])
            };
            Chunks[x, y, z].UpdateBorder(borders, solid, false);
        }
    }
}
