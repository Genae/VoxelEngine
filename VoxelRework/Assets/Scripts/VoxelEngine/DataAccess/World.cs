using Assets.Scripts.VoxelEngine.Containers.Chunks;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.DataAccess
{
    public class World
    {
        private readonly MaterialCollection _materialCollection;
        public ChunkCloud SolidChunks;
        public FluidChunkCloud FluidChunks;

        public World(MaterialCollection materialCollection)
        {
            _materialCollection = materialCollection;
            var go = new GameObject("map");
            SolidChunks = new ChunkCloud(_materialCollection, go.transform);
            FluidChunks = new FluidChunkCloud(_materialCollection, go.transform);
        }

        public void SetSlice(int slice)
        {
            SolidChunks.SetSlice(slice);
            FluidChunks.SetSlice(slice);
        }

        public void StartBatch()
        {
            SolidChunks.StartBatch();
            FluidChunks.StartBatch();
        }
        public void FinishBatch()
        {
            SolidChunks.FinishBatch();
            FluidChunks.FinishBatch();
        }

        public void SetVoxel(VoxelMaterial mat, Vector3Int pos)
        {
            if (mat.Fluid)
            {
                FluidChunks.SetVoxel(mat, pos);
            }
            else
            {
                SolidChunks.SetVoxel(mat, pos);
            }
        }
    }
}
