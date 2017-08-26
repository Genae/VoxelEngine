using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Data.Map
{
    public class ChunkData : ContainerData
    {
        public ChunkBorder[] ChunkBorders;
        public LocalAStarNetwork LocalAStar = new LocalAStarNetwork();
        private readonly List<Multiblock.Multiblock> _multiblocks = new List<Multiblock.Multiblock>();
        private readonly Dictionary<Vector3, Multiblock.Multiblock> _smallMultiblocks = new Dictionary<Vector3, Multiblock.Multiblock>();

        public ChunkData(Vector3 position) : base(Chunk.ChunkSize, position)
        {
            ChunkBorders = new ChunkBorder[6];
            for (int i = 0; i < 6; i++)
            {
                ChunkBorders[i] = new ChunkBorder(this, i);
            }
            OnContainerUpdated();
        }
        
        public override void SetVoxel(int x, int y, int z, VoxelMaterial material)
        {
            base.SetVoxel(x, y, z, material);
            Vector3 pos;
            if (!material.Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")))
            {
                pos = new Vector3(x, y - 1, z);
                if (_smallMultiblocks.ContainsKey(pos))
                {
                    var mb = _smallMultiblocks[pos];
                    _smallMultiblocks.Remove(pos);
                    Object.Destroy(mb.gameObject);
                }
            }

            pos = new Vector3(x, y, z);
            if (_smallMultiblocks.ContainsKey(pos))
            {
                var mb = _smallMultiblocks[pos];
                _smallMultiblocks.Remove(pos);
                Object.Destroy(mb.gameObject);
            }
            OnContainerUpdated();
        }

        public void BorderUpdate(int side)
        {
            OnContainerUpdated();
        }

        public void CheckDirtyVoxels()
        {
            if (DirtyVoxels.Count == 0)
                return;
            if (DirtyVoxels.Any(v => (int)v.x == 0))
                ChunkBorders[0].UpdateBorder();
            if (DirtyVoxels.Any(v => (int)v.x == Chunk.ChunkSize - 1))
                ChunkBorders[1].UpdateBorder();
            if (DirtyVoxels.Any(v => (int)v.y == 0))
                ChunkBorders[2].UpdateBorder();
            if (DirtyVoxels.Any(v => (int)v.y == Chunk.ChunkSize - 1))
                ChunkBorders[3].UpdateBorder();
            if (DirtyVoxels.Any(v => (int)v.z == 0))
                ChunkBorders[4].UpdateBorder();
            if (DirtyVoxels.Any(v => (int)v.z == Chunk.ChunkSize - 1))
                ChunkBorders[5].UpdateBorder();
            OnContainerUpdated();
            DirtyVoxels.Clear();
        }

        public override bool IsWorldPosBlocked(int x, int y, int z)
        {
            var posX = (int)Position.x + x % Chunk.ChunkSize;
            var posY = (int)Position.y + y % Chunk.ChunkSize;
            var posZ = (int)Position.z + z % Chunk.ChunkSize;

            return base.IsWorldPosBlocked(posX, posY, posZ) || _multiblocks.Any(m => m.ContainerData.IsWorldPosBlocked(posX, posY, posZ));
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

        public void MineVoxel(int x, int y, int z, Inventory inventory)
        {
            var type = GetVoxelType(x, y, z);
            if (type.Drops != null)
            {
                foreach (var drop in type.Drops)
                {
                    var item = ItemManager.GetItemType(drop.Item);
                    var notStored = inventory.InsertItems(item, drop.Amount);
                    for (var i = 0; i < notStored; i++)
                    {
                        ItemManager.DropItem(new Vector3(x, y, z) + Position, item);
                    }
                }
            }
            SetVoxel(x, y, z, MaterialRegistry.Instance.GetMaterialFromName("Air"));
        }

        public bool[][,] GetNeighbourBorders()
        {
            var borders = new bool[6][,];
            for (var i = 0; i < 6; i++)
                borders[i % 2 == 0 ? i + 1 : i - 1] = ChunkBorders[i].GetNeighbourBorder();
            return borders;
        }
    }

    public class ChunkBorder
    {
        public bool IsSolid;
        public bool[,] VoxelAtPositionSolid;
        public ChunkData MyChunk;
        public ChunkData NeighbourChunk;
        public int Side;

        public ChunkBorder(ChunkData myChunk, int side)
        {
            MyChunk = myChunk;
            Side = side;
            UpdateBorder();
        }

        public void UpdateBorder()
        {
            VoxelAtPositionSolid = new bool[Chunk.ChunkSize, Chunk.ChunkSize];
            for (var a = 0; a < Chunk.ChunkSize; a++)
            {
                for (var b = 0; b < Chunk.ChunkSize; b++)
                {
                    VoxelAtPositionSolid[a, b] = MyChunk.GetVoxelActive(SideToOuterPlane(Side, a, b));
                    IsSolid = VoxelAtPositionSolid[a, b] && IsSolid;
                }
            }
            UpdateNeighbour();
        }

        public bool[,] GetNeighbourBorder()
        {
            if(GetNeighbourChunk() == null)
                return new bool[Chunk.ChunkSize,Chunk.ChunkSize];
            return NeighbourChunk.ChunkBorders[Side % 2 == 0 ? Side + 1 : Side - 1].VoxelAtPositionSolid;
        }
        public bool IsNeighbourSolid()
        {
            if (GetNeighbourChunk() == null)
                return false;
            return NeighbourChunk.ChunkBorders[Side % 2 == 0 ? Side + 1 : Side - 1].IsSolid;
        }

        private void UpdateNeighbour()
        {
            var neighbour = GetNeighbourChunk();
            if (neighbour != null)
                neighbour.BorderUpdate(Side % 2 == 0 ? Side + 1 : Side - 1);
        }

        private ChunkData GetNeighbourChunk()
        {
            return NeighbourChunk ?? (NeighbourChunk = World.At(MyChunk.Position + SideToDirection(Side) * Chunk.ChunkSize).GetChunkData());
        }

        private static Vector3 SideToOuterPlane(int side, float a, float b)
        {
            switch (side)
            {
                case 0: return new Vector3(Chunk.ChunkSize-1, a, b);
                case 1: return new Vector3(0, a, b);
                case 2: return new Vector3(a, Chunk.ChunkSize - 1, b);
                case 3: return new Vector3(a, 0, b);
                case 4: return new Vector3(a, b, Chunk.ChunkSize - 1);
                case 5: return new Vector3(a, b, 0);
            }
            return Vector3.zero;
        }
        private static Vector3 SideToDirection(int side)
        {
            switch (side)
            {
                case 0: return Vector3.right;
                case 1: return Vector3.left;
                case 2: return Vector3.up;
                case 3: return Vector3.down;
                case 4: return Vector3.forward;
                case 5: return Vector3.back;
            }
            return Vector3.zero;
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
        
        public virtual void SetVoxel(int x, int y, int z, VoxelMaterial material)
        {
            var type = MaterialRegistry.Instance.GetMaterialId(material);
            if (Voxels[x, y, z] == null && type == 0 || Voxels[x, y, z] != null && type == Voxels[x, y, z].BlockType)
                return;
            if (type == 0) // set to air
            {
                Voxels[x, y, z] = null;
            }
            else
            {
                if (Voxels[x, y, z] == null)
                {
                    Voxels[x, y, z] = new VoxelData(type);
                }
                else
                {
                    Voxels[x, y, z].BlockType = type;
                }
            }
            DirtyVoxels.Add(new Vector3(x, y, z));
        }
        
        public virtual bool IsWorldPosBlocked(int x, int y, int z)
        {
            var posFrom = (new Vector3(x, y, z) - Position) / Scale;
            var posTo = Scale < 1 ? posFrom + new Vector3(1/Scale-1, 1 / Scale - 1, 1/Scale-1) : posFrom;
            for (var dx = Mathf.Max(0, (int)posTo.x); dx <= Mathf.Min((int)posFrom.x, Size-1); dx++)
            {
                for (var dy = Mathf.Max(0, (int)posTo.y); dy <= Mathf.Min((int)posFrom.y, Size-1); dy++)
                {
                    for (var dz = Mathf.Max(0, (int)posTo.z); dz <= Mathf.Min((int)posFrom.z, Size-1); dz++)
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

        public bool GetVoxelActive(Vector3I v)
        {
            return GetVoxelActive(v.x, v.y, v.z);
        }

        public bool GetVoxelActive(int x, int y, int z)
        {
            return Voxels[x, y, z] != null && Voxels[x, y, z].IsActive;
        }
    }
}
