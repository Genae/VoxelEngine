using System;

namespace Assets.Scripts.Base.Data.Map
{
    public class ChunkData
    {
        public const int ChunkSize = 16;
        public VoxelData[,,] Voxels;
        public bool[][,] NeighbourBorders;
        public bool[] NeighbourSolidBorders;

        public Action<ChunkData> ChunkUpdated;


        public void UpdateBorder(bool[,] border, bool solid, int side, bool runUpdate = true)
        {
            NeighbourBorders[side] = border;
            NeighbourSolidBorders[side] = solid;
            if (runUpdate)
                OnChunkUpdated();
        }

        private void OnChunkUpdated()
        {
            if(ChunkUpdated != null)
                ChunkUpdated(this);
        }

        public void UpdateBorder(bool[][,] border, bool[] solid, bool runUpdate = true)
        {
            NeighbourBorders = border;
            NeighbourSolidBorders = solid;
            for (int i = 0; i < 6; i++)
            {
                if (NeighbourBorders[i] == null)
                    NeighbourBorders[i] = new bool[ChunkSize, ChunkSize];
            }
            if (runUpdate)
                OnChunkUpdated();
        }

        public bool HasSolidBorder(int dir, out bool[,] border)
        {
            border = new bool[ChunkSize, ChunkSize];
            var solid = true;
            switch (dir)
            {
                case 1: //+x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[y, z] = Voxels[ChunkSize - 1, y, z].IsActive;
                            solid = solid && Voxels[ChunkSize - 1, y, z].IsActive;
                        }
                    }
                    return solid;
                case 2: //-x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[y, z] = Voxels[0, y, z].IsActive;
                            solid = solid && Voxels[0, y, z].IsActive;
                        }
                    }
                    return solid;
                case 3: //+y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, ChunkSize - 1, z].IsActive;
                            solid = solid && Voxels[x, ChunkSize - 1, z].IsActive;
                        }
                    }
                    return solid;
                case 4: //-y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, 0, z].IsActive;
                            solid = solid && Voxels[x, 0, z].IsActive;
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            border[x, y] = Voxels[x, y, ChunkSize - 1].IsActive;
                            solid = solid && Voxels[x, y, ChunkSize - 1].IsActive;
                        }
                    }
                    return solid;
                case 6: //-z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            border[x, y] = Voxels[x, y, 0].IsActive;
                            solid = solid && Voxels[x, y, 0].IsActive;
                        }
                    }
                    return solid;
            }
            return false;
        }
    }
}
