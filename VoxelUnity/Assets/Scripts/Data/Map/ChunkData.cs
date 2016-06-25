using System;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class ChunkData
    {
        public VoxelData[,,] Voxels;
        public bool[][,] NeighbourBorders;
        public bool[] NeighbourSolidBorders;
        private ChunkData[] _neighbourData;

        public Action<ChunkData> ChunkUpdated;

        public ChunkData()
        {
            Voxels = new VoxelData[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkSize];
        }

        public void UpdateBorder(bool[,] border, bool solid, int side, bool runUpdate = true)
        {
            NeighbourBorders[side] = border;
            NeighbourSolidBorders[side] = solid;
            if (runUpdate)
                OnChunkUpdated();
        }

        public void VoxelUpdated(Vector3 position)
        {
            if ((int) position.x == 0)
                UpdateNeighbour(0);
            if ((int)position.x == Chunk.ChunkSize-1)
                UpdateNeighbour(1);
            if ((int)position.y == 0)
                UpdateNeighbour(2);
            if ((int)position.y == Chunk.ChunkSize - 1)
                UpdateNeighbour(3);
            if ((int)position.z == 0)
                UpdateNeighbour(4);
            if ((int)position.z == Chunk.ChunkSize - 1)
                UpdateNeighbour(5);
            OnChunkUpdated();
        }

        private void UpdateNeighbour(int side)
        {
            if (_neighbourData[side] == null)
                return;
            bool[,] border;
            var solid = HasSolidBorder(side, out border);
            _neighbourData[side].UpdateBorder(border, solid, side%2==0?side+1:side-1);
        }

        private void OnChunkUpdated()
        {
            if(ChunkUpdated != null)
                ChunkUpdated(this);
        }

        public void UpdateBorder(bool[][,] border, bool[] solid, ChunkData[] neighbourData, bool runUpdate = true)
        {
            _neighbourData = neighbourData;
            NeighbourBorders = border;
            NeighbourSolidBorders = solid;
            for (var i = 0; i < 6; i++)
            {
                if (NeighbourBorders[i] == null)
                    NeighbourBorders[i] = new bool[Chunk.ChunkSize, Chunk.ChunkSize];
            }
            if (runUpdate)
                OnChunkUpdated();
        }

        public bool HasSolidBorder(int dir, out bool[,] border)
        {
            border = new bool[Chunk.ChunkSize, Chunk.ChunkSize];
            var solid = true;
            switch (dir)
            {
                case 1: //+x
                    for (var y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[y, z] = Voxels[Chunk.ChunkSize - 1, y, z].IsActive;
                            solid = solid && Voxels[Chunk.ChunkSize - 1, y, z].IsActive;
                        }
                    }
                    return solid;
                case 2: //-x
                    for (var y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[y, z] = Voxels[0, y, z].IsActive;
                            solid = solid && Voxels[0, y, z].IsActive;
                        }
                    }
                    return solid;
                case 3: //+y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, Chunk.ChunkSize - 1, z].IsActive;
                            solid = solid && Voxels[x, Chunk.ChunkSize - 1, z].IsActive;
                        }
                    }
                    return solid;
                case 4: //-y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, 0, z].IsActive;
                            solid = solid && Voxels[x, 0, z].IsActive;
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
                        {
                            border[x, y] = Voxels[x, y, Chunk.ChunkSize - 1].IsActive;
                            solid = solid && Voxels[x, y, Chunk.ChunkSize - 1].IsActive;
                        }
                    }
                    return solid;
                case 6: //-z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
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
