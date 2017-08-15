using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Material;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Data.Map
{
    public class ChunkData : ContainerData
    {
        public bool[][,] NeighbourBorders;
        public bool[] NeighbourSolidBorders;
        public ChunkData[] NeighbourData;
        public LocalAStarNetwork LocalAStar = new LocalAStarNetwork();
        private readonly List<Multiblock.Multiblock> _multiblocks = new List<Multiblock.Multiblock>();
        private readonly Dictionary<Vector3, Multiblock.Multiblock> _smallMultiblocks = new Dictionary<Vector3, Multiblock.Multiblock>();


        public ChunkData(Vector3 position) : base(Chunk.ChunkSize, position)
        {
        }

        public override void SetVoxelType(int x, int y, int z, VoxelMaterial material)
        {
            base.SetVoxelType(x, y, z, material);
            var pos = material.Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")) ? new Vector3(x, y, z) : new Vector3(x, y - 1 , z);

            if (_smallMultiblocks.ContainsKey(pos))
            {
                var mb = _smallMultiblocks[pos];
                _smallMultiblocks.Remove(pos);
                Object.Destroy(mb.gameObject);
            }
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
            if (DirtyVoxels.Any(v => (int)v.x == Chunk.ChunkSize - 1))
                UpdateNeighbour(1);
            if (DirtyVoxels.Any(v => (int)v.y == 0))
                UpdateNeighbour(2);
            if (DirtyVoxels.Any(v => (int)v.y == Chunk.ChunkSize - 1))
                UpdateNeighbour(3);
            if (DirtyVoxels.Any(v => (int)v.z == 0))
                UpdateNeighbour(4);
            if (DirtyVoxels.Any(v => (int)v.z == Chunk.ChunkSize - 1))
                UpdateNeighbour(5);
            OnContainerUpdated();
            DirtyVoxels.Clear();
        }

        private void UpdateNeighbour(int side)
        {
            if (NeighbourData[side] == null)
                return;
            bool[,] border;
            var solid = HasSolidBorder(side, out border);
            NeighbourData[side].UpdateBorder(border, solid, side%2==0?side+1:side-1);
        }


        public void UpdateBorder(bool[][,] border, bool[] solid, ChunkData[] neighbourData, bool runUpdate = true)
        {
            NeighbourData = neighbourData;
            NeighbourBorders = border;
            NeighbourSolidBorders = solid;
            for (var i = 0; i < 6; i++)
            {
                if (NeighbourBorders[i] == null)
                    NeighbourBorders[i] = new bool[Chunk.ChunkSize, Chunk.ChunkSize];
            }
            if (runUpdate)
                OnContainerUpdated();
        }

        public override bool IsWorldPosBlocked(int x, int y, int z)
        {
            var posX = (int)Position.x + x % Chunk.ChunkSize;
            var posY = (int)Position.y + y % Chunk.ChunkSize;
            var posZ = (int)Position.z + z % Chunk.ChunkSize;

            return base.IsWorldPosBlocked(posX, posY, posZ) || _multiblocks.Any(m => m.ContainerData.IsWorldPosBlocked(posX, posY, posZ));
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
                            border[y, z] = GetVoxelActive(Chunk.ChunkSize - 1, y, z);
                            solid = solid && GetVoxelActive(Chunk.ChunkSize - 1, y, z);
                        }
                    }
                    return solid;
                case 2: //-x
                    for (var y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[y, z] = GetVoxelActive(0, y, z);
                            solid = solid && GetVoxelActive(0, y, z);
                        }
                    }
                    return solid;
                case 3: //+y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = GetVoxelActive(x, Chunk.ChunkSize - 1, z);
                            solid = solid && GetVoxelActive(x, Chunk.ChunkSize - 1, z);
                        }
                    }
                    return solid;
                case 4: //-y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = GetVoxelActive(x, 0, z);
                            solid = solid && GetVoxelActive(x, 0, z);
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, Chunk.ChunkSize - 1);
                            solid = solid && GetVoxelActive(x, y, Chunk.ChunkSize - 1);
                        }
                    }
                    return solid;
                case 6: //-z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, 0);
                            solid = solid && GetVoxelActive(x, y, 0);
                        }
                    }
                    return solid;
            }
            return false;
        }

        public void AttachMultiblock(Multiblock.Multiblock m)
        {
            if (!_multiblocks.Contains(m))
            {
                _multiblocks.Add(m);
                OnContainerUpdated();
            }
        }

        public void RegisterSmallMultiblock(Multiblock.Multiblock mb, Vector3 pos)
        {
            if (_smallMultiblocks.ContainsKey(pos))
            {
                Object.Destroy(_smallMultiblocks[pos].gameObject);
            }
            _smallMultiblocks[pos] = mb;
        }
    }

    public class ContainerData
    {
        protected readonly List<Vector3> DirtyVoxels = new List<Vector3>();
        public VoxelData[,,] Voxels;
        public int Size;
        public float Scale;
        public Vector3 Position;

        public Action ContainerUpdated;

        public ContainerData(int size, Vector3 position, float scale = 1f)
        {
            Size = size;
            Scale = scale;
            Position = position;
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
        
        public virtual void SetVoxelType(int x, int y, int z, VoxelMaterial material)
        {
            var type = MaterialRegistry.Instance.GetMaterialId(material);
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

        public virtual bool IsWorldPosBlocked(int x, int y, int z)
        {
            var posFrom = (new Vector3(x, y, z) - Position) / Scale;
            var posTo = Scale < 1 ? posFrom + new Vector3((1/Scale)-1, (1 / Scale) - 1, (1/Scale)-1) : posFrom;
            for (var dx = Mathf.Max(0, (int)posTo.x); dx <= Mathf.Min((int)(posFrom.x), Size-1); dx++)
            {
                for (var dy = Mathf.Max(0, (int)posTo.y); dy <= Mathf.Min((int)(posFrom.y), Size-1); dy++)
                {
                    for (var dz = Mathf.Max(0, (int)posTo.z); dz <= Mathf.Min((int)(posFrom.z), Size-1); dz++)
                    {
                        if (Map.Instance.IsInBounds(x, y, z) && !GetVoxelType(dx, dy, dz).Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")))
                            return true;
                    }
                }
            }
            return false;
        }

        public VoxelMaterial GetVoxelType(int x, int y, int z)
        {
            return MaterialRegistry.Instance.MaterialFromId(Voxels[x, y, z] == null ? 0 : Voxels[x, y, z].BlockType);
        }

        public bool GetVoxelActive(int x, int y, int z)
        {
            return Voxels[x, y, z] != null && Voxels[x, y, z].IsActive;
        }
        
        public VoxelData SetVoxel(int x, int y, int z, bool active, VoxelMaterial material)
        {
            return Voxels[x, y, z] = new VoxelData(active, MaterialRegistry.Instance.GetMaterialId(material));
        }

        public VoxelData SetEntityVoxel(int x, int y, int z, bool active, int colorTyp)
        {
            return Voxels[x, y, z] = new VoxelData(active, colorTyp);
        }
    }
}
