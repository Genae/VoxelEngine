using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Data.Map
{
    public class ChunkData : ContainerData
    {
        public bool[][,] NeighbourBorders = new bool[6][,];
        public bool[] NeighbourSolidBorders = new bool[6];
        public ChunkData[] NeighbourData;
        public LocalAStarNetwork LocalAStar = new LocalAStarNetwork();
        private readonly List<Multiblock.Multiblock> _multiblocks = new List<Multiblock.Multiblock>();
        private readonly Dictionary<Vector3, Multiblock.Multiblock> _smallMultiblocks = new Dictionary<Vector3, Multiblock.Multiblock>();


        public ChunkData(Vector3 position) : base(Chunk.ChunkSize, position)
        {
            var chunkPos = Position/Chunk.ChunkSize;
            NeighbourData = new[]
            {
                World.At((chunkPos + new Vector3(1, 0, 0))*Chunk.ChunkSize).GetChunkData(),
                World.At((chunkPos + new Vector3(-1, 0, 0))*Chunk.ChunkSize).GetChunkData(),
                World.At((chunkPos + new Vector3(0, 1, 0))*Chunk.ChunkSize).GetChunkData(),
                World.At((chunkPos + new Vector3(0, -1, 0))*Chunk.ChunkSize).GetChunkData(),
                World.At((chunkPos + new Vector3(0, 0, 1))*Chunk.ChunkSize).GetChunkData(),
                World.At((chunkPos + new Vector3(0, 0, 1))*Chunk.ChunkSize).GetChunkData()
            };
            for (var i = 0; i < NeighbourData.Length; i++)
            {
                if (NeighbourData[i] == null)
                {
                    NeighbourBorders[i] = new bool[Chunk.ChunkSize,Chunk.ChunkSize];
                }
                else
                {
                    NeighbourData[i].ConnectNeigbour(this);
                    bool[,] border;
                    var solid = NeighbourData[i].HasSolidBorder(i % 2 == 0 ? i + 1 : i - 1, out border);
                    UpdateBorder(border, solid, i);
                }
            }
            OnContainerUpdated();
        }

        private void ConnectNeigbour(ChunkData chunkData)
        {
            var diff = Position - chunkData.Position;
            if (diff.x < 0)
            {
                NeighbourData[0] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(1, out border);
                UpdateBorder(border, solid, 0);
            }
            if (diff.x > 0)
            {
                NeighbourData[1] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(0, out border);
                UpdateBorder(border, solid, 1);
            }
            if (diff.y < 0)
            {
                NeighbourData[2] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(3, out border);
                UpdateBorder(border, solid, 2);
            }
            if (diff.y > 0)
            {
                NeighbourData[3] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(2, out border);
                UpdateBorder(border, solid, 3);
            }
            if (diff.z < 0)
            {
                NeighbourData[4] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(5, out border);
                UpdateBorder(border, solid, 4);
            }
            if (diff.z > 0)
            {
                NeighbourData[5] = chunkData;
                bool[,] border;
                var solid = chunkData.HasSolidBorder(4, out border);
                UpdateBorder(border, solid, 5);
            }
        }

        public override void SetVoxelType(int x, int y, int z, VoxelMaterial material)
        {
            base.SetVoxelType(x, y, z, material);
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

        public void UpdateBorder(bool[,] border, bool solid, int side)
        {
            Debug.Log(border[0, 0] + "/" +  solid);
            NeighbourBorders[side] = border;
            NeighbourSolidBorders[side] = solid;
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
            if (NeighbourData == null || NeighbourData[side] == null)
            {
                //TODO error
                return;
            }
            bool[,] border;
            var solid = HasSolidBorder(side, out border);
            NeighbourData[side].UpdateBorder(border, solid, side%2==0?side+1:side-1);
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
                case 0: //+x
                    for (var y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[y, z] = GetVoxelActive(Chunk.ChunkSize - 1, y, z);
                            solid = border[y, z] && solid;
                        }
                    }
                    return solid;
                case 1: //-x
                    for (var y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[y, z] = GetVoxelActive(0, y, z);
                            solid = border[y, z] && solid;
                        }
                    }
                    return solid;
                case 2: //+y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = GetVoxelActive(x, Chunk.ChunkSize - 1, z);
                            solid = border[x, z] && solid;
                        }
                    }
                    return solid;
                case 3: //-y
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var z = 0; z < Chunk.ChunkSize; z++)
                        {
                            border[x, z] = GetVoxelActive(x, 0, z);
                            solid = border[x, z] && solid;
                        }
                    }
                    return solid;
                case 4: //+z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, Chunk.ChunkSize - 1);
                            solid = border[x, y] && solid;
                        }
                    }
                    return solid;
                case 5: //-z
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (var y = 0; y < Chunk.ChunkSize; y++)
                        {
                            border[x, y] = GetVoxelActive(x, y, 0);
                            solid = border[x, y] && solid;
                        }
                    }
                    return solid;
                default:
                    return false;
            }
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
            SetVoxelType(x, y, z, MaterialRegistry.Instance.GetMaterialFromName("Air"));
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
        
        public VoxelData SetVoxel(int x, int y, int z, VoxelMaterial material)
        {
            return Voxels[x, y, z] = new VoxelData(MaterialRegistry.Instance.GetMaterialId(material));
        }
    }
}
