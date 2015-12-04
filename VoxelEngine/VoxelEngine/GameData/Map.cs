using System;
using OpenTK;
using VoxelEngine.Shaders;

namespace VoxelEngine.GameData
{
    public class Map
    {
        public Chunk[,,] Chunks;
        public Shader Shader;

        public Map(int size, int height)
        {
            Chunks = new Chunk[size, height, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        Chunks[x, y, z] = new Chunk(new Vector3(x,y,z));
                    }
                }
            }
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        Chunks[x, y, z].SetActive(IsChunkActive(x, y, z));
                    }
                }
            }
        }
        /*
        private int fboID;
        private int depthTextureID;

        private void generateShadowFBO()
        {
            int shadowMapWidth = 200;
            int shadowMapHeight = 200;

            GL.GenTextures(0, out depthTextureID);
            GL.BindTexture(TextureTarget.Texture2D, depthTextureID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameterName.TextureMinFilter);
        }*/

        public void LoadHeightmap(short[,] heightmap)
        {
            for (int x = 0; x < Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (int z = 0; z < Chunks.GetLength(2) * Chunk.ChunkSize; z++)
                {
                    var height = heightmap[x, z];
                    for (int y = 0; y < height; y++)
                    {
                        GetVoxel(x, y, z).IsActive = true;
                    }
                }
            }
        }

        public void LoadHeightmap(float[,] heightmap, short maxHeight)
        {
            var v = 0;
            for (int x = 0; x < Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (int z = 0; z < Chunks.GetLength(2) * Chunk.ChunkSize; z++)
                {
                    var height = heightmap[x, z]* maxHeight;
                    for (int y = 0; y < height; y++)
                    {
                        GetVoxel(x, y, z).IsActive = true;
                        v++;
                    }
                }
            }
            foreach (var chunk in Chunks)
            {
                chunk.OnChunkUpdated();
            }
            Console.WriteLine(v);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            var cx = x/Chunk.ChunkSize;
            var cy = y/Chunk.ChunkSize;
            var cz = z/Chunk.ChunkSize;
            return Chunks[cx, cy, cz].Voxels[x%Chunk.ChunkSize, y%Chunk.ChunkSize, z%Chunk.ChunkSize];
        }

        private bool IsChunkActive(int x, int y , int z)
        {
            return x == 0 || x == Chunks.GetLength(0) - 1 || !Chunks[x - 1, y, z].HasSolidBorder(1) || !Chunks[x + 1, y, z].HasSolidBorder(2) ||
                   y == 0 || y == Chunks.GetLength(1) - 1 || !Chunks[x, y - 1, z].HasSolidBorder(3) || !Chunks[x, y + 1, z].HasSolidBorder(4) ||
                   z == 0 || z == Chunks.GetLength(2) - 1 || !Chunks[x, y, z - 1].HasSolidBorder(5) || !Chunks[x, y, z + 1].HasSolidBorder(6);
        }
    }
}
