using System.Collections;
using AccessLayer.Material;
using AccessLayer.Worlds;
using EngineLayer.Voxels.Containers.Chunks;

namespace EngineLayer.Voxels.Data
{
    public class MapData
    {
        public ChunkData[,,] Chunks { get; }
        private readonly float Scale;
        
        public MapData(int size, int height, float scaleMultiplier)
        {
            Chunks = new ChunkData[(int)(size * scaleMultiplier), (int)(height * scaleMultiplier), (int)(size * scaleMultiplier)];
            Scale = scaleMultiplier;
        }

        public IEnumerator LoadHeightmap(float[,] heightmap, float[,] bottom, float[,] cut, float heightmapHeight)
        {
            var grass = MaterialRegistry.Instance.GetMaterialFromName("Grass");
            var dirt = MaterialRegistry.Instance.GetMaterialFromName("Dirt");
            var stone = MaterialRegistry.Instance.GetMaterialFromName("Stone");
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
                        var blockType = y == (int) lheight - 1 ? grass : (y >= (int) lheight - 4 ? dirt : stone);
                        World.At(x, y, z).SetVoxel(blockType);
                    }
                }
                yield return null;
            }
        }
    }
}
