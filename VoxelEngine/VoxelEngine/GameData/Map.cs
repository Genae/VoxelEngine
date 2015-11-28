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
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            foreach (var chunk in Chunks)
            {
                chunk.OnRenderFrame(e);
            }
        }
    }
}
