using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class ChunkData : ContainerData
    {
        public bool[][,] NeighbourBorders;
        public bool[] NeighbourSolidBorders;
        private ChunkData[] _neighbourData;


        public ChunkData() : base(Chunk.ChunkSize2)
        {
        }

        public void UpdateBorder(bool[,] border, bool solid, int side, bool runUpdate = true)
        {
            NeighbourBorders[side] = border;
            NeighbourSolidBorders[side] = solid;
            if (runUpdate)
                OnContainerUpdated();
        }
        
        public override void CheckDirtyVoxels()
        {
            if (DirtyVoxels.Count == 0)
                return;
            if (DirtyVoxels.Any(v => (int)v.x == 0))
                UpdateNeighbour(0);
            if (DirtyVoxels.Any(v => (int)v.x == Chunk.ChunkSize2 - 1))
                UpdateNeighbour(1);
            if (DirtyVoxels.Any(v => (int)v.y == 0))
                UpdateNeighbour(2);
            if (DirtyVoxels.Any(v => (int)v.y == Chunk.ChunkSize2 - 1))
                UpdateNeighbour(3);
            if (DirtyVoxels.Any(v => (int)v.z == 0))
                UpdateNeighbour(4);
            if (DirtyVoxels.Any(v => (int)v.z == Chunk.ChunkSize2 - 1))
                UpdateNeighbour(5);
            OnContainerUpdated();
            DirtyVoxels.Clear();
        }

        private void UpdateNeighbour(int side)
        {
            if (_neighbourData[side] == null)
                return;
            bool[,] border;
            var solid = HasSolidBorder(side, out border);
            _neighbourData[side].UpdateBorder(border, solid, side%2==0?side+1:side-1);
        }


        public void UpdateBorder(bool[][,] border, bool[] solid, ChunkData[] neighbourData, bool runUpdate = true)
        {
            _neighbourData = neighbourData;
            NeighbourBorders = border;
            NeighbourSolidBorders = solid;
            for (var i = 0; i < 6; i++)
            {
                if (NeighbourBorders[i] == null)
                    NeighbourBorders[i] = new bool[Chunk.ChunkSize2, Chunk.ChunkSize2];
            }
            if (runUpdate)
                OnContainerUpdated();
        }

        public bool HasSolidBorder(int dir, out bool[,] border)
        {
            border = new bool[Chunk.ChunkSize2, Chunk.ChunkSize2];
            var solid = true;
            switch (dir)
            {
                case 1: //+x
                    for (var y = 0; y < Chunk.ChunkSize2; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize2; z++)
                        {
                            border[y, z] = GetVoxelActive(Chunk.ChunkSize2 - 1, y, z);
                            solid = solid && GetVoxelActive(Chunk.ChunkSize2 - 1, y, z);
                        }
                    }
                    return solid;
                case 2: //-x
                    for (var y = 0; y < Chunk.ChunkSize2; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize2; z++)
                        {
                            border[y, z] = GetVoxelActive(0, y, z);
                            solid = solid && GetVoxelActive(0, y, z);
                        }
                    }
                    return solid;
                case 3: //+y
                    for (var x = 0; x < Chunk.ChunkSize2; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize2; z++)
                        {
                            border[x, z] = GetVoxelActive(x, Chunk.ChunkSize2 - 1, z);
                            solid = solid && GetVoxelActive(x, Chunk.ChunkSize2 - 1, z);
                        }
                    }
                    return solid;
                case 4: //-y
                    for (var x = 0; x < Chunk.ChunkSize2; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize2; z++)
                        {
                            border[x, z] = GetVoxelActive(x, 0, z);
                            solid = solid && GetVoxelActive(x, 0, z);
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < Chunk.ChunkSize2; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize2; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, Chunk.ChunkSize2 - 1);
                            solid = solid && GetVoxelActive(x, y, Chunk.ChunkSize2 - 1);
                        }
                    }
                    return solid;
                case 6: //-z
                    for (var x = 0; x < Chunk.ChunkSize2; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize2; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, 0);
                            solid = solid && GetVoxelActive(x, y, 0);
                        }
                    }
                    return solid;
            }
            return false;
        }
    }

    public class ContainerData
    {
        protected readonly List<Vector3> DirtyVoxels = new List<Vector3>();
        protected VoxelData[,,] Voxels;
        public int Size;

        public Action ContainerUpdated;

        public ContainerData(int size)
        {
            Size = size;
            Voxels = new VoxelData[size, size, size];
        }

        protected void OnContainerUpdated()
        {
            if (ContainerUpdated != null)
                ContainerUpdated();
        }

        public virtual void CheckDirtyVoxels()
        {
            if (DirtyVoxels.Count == 0)
                return;
            OnContainerUpdated();
            DirtyVoxels.Clear();
        }
        
        public void SetVoxelType(int x, int y, int z, VoxelMaterial material)
        {
            var type = MaterialRegistry.GetMaterialId(material);
            if ((Voxels[x, y, z] == null && type == 0) || (Voxels[x, y, z] != null && type == Voxels[x, y, z].BlockType))
                return;
            if (type == 0) // set to air
            {
                Voxels[x, y, z] = null;
            }
            else
            {
                if (Voxels[x, y, z] == null)
                {
                    Voxels[x, y, z] = new VoxelData(true, type);
                }
                else
                {
                    Voxels[x, y, z].BlockType = type;
                }
            }
            DirtyVoxels.Add(new Vector3(x, y, z));
        }

        public void SetVoxelActive(int x, int y, int z, bool active)
        {
            if ((Voxels[x, y, z] == null) || (Voxels[x, y, z] != null && active == Voxels[x, y, z].IsActive))
                return;
            Voxels[x, y, z].IsActive = active;
            DirtyVoxels.Add(new Vector3(x, y, z));
        }

        public VoxelMaterial GetVoxelType(int x, int y, int z)
        {
            return MaterialRegistry.MaterialFromId(Voxels[x, y, z] == null ? 0 : Voxels[x, y, z].BlockType);
        }

        public bool GetVoxelActive(int x, int y, int z)
        {
            return Voxels[x, y, z] != null && Voxels[x, y, z].IsActive;
        }
        
        public VoxelData SetVoxel(int x, int y, int z, bool active, VoxelMaterial material)
        {
            return Voxels[x, y, z] = new VoxelData(active, MaterialRegistry.GetMaterialId(material));
        }
    }
}
