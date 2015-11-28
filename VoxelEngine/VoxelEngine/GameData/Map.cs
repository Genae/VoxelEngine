using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace VoxelEngine.GameData
{
    public class Map
    {
        public Chunk[,,] Chunks;

        public Map(int size)
        {
            Chunks = new Chunk[size, size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
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

        public void OnRenderFrame(FrameEventArgs e)
        {
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        Chunks[x, y, z].OnRenderFrame(e);
                    }
                }
            }
        }

        private bool IsChunkActive(int x, int y , int z)
        {
            return x == 0 || x == Chunks.GetLength(0) - 1 || !Chunks[x - 1, y, z].HasSolidBorder(1) || !Chunks[x + 1, y, z].HasSolidBorder(2) ||
                   y == 0 || y == Chunks.GetLength(1) - 1 || !Chunks[x, y - 1, z].HasSolidBorder(3) || !Chunks[x, y + 1, z].HasSolidBorder(4) ||
                   z == 0 || z == Chunks.GetLength(2) - 1 || !Chunks[x, y, z - 1].HasSolidBorder(5) || !Chunks[x, y, z + 1].HasSolidBorder(6);
        }
    }
}
