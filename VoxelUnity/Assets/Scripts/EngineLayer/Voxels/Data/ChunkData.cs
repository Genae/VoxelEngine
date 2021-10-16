using System.Collections.Generic;
using System.Linq;
using AccessLayer;
using AccessLayer.Material;
using EngineLayer.Objects;
using EngineLayer.Voxels.Containers.Chunks;
using EngineLayer.Voxels.Containers.Multiblock;
using EngineLayer.Voxels.Material;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineLayer.Voxels.Data
{
    public class ChunkData : ContainerData
    {
        public readonly ChunkBorder[] ChunkBorders;
        public readonly LocalAStarNetwork LocalAStar = new LocalAStarNetwork();
        private readonly List<Multiblock> _multiblocks = new List<Multiblock>();
        private readonly Dictionary<Vector3, Multiblock> _smallMultiblocks = new Dictionary<Vector3, Multiblock>();

        public ChunkData(Vector3 position) : base(Chunk.ChunkSize, position)
        {
            ChunkBorders = new ChunkBorder[6];
            for (var i = 0; i < 6; i++)
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

        public void AttachMultiblock(Multiblock m)
        {
            if (!_multiblocks.Contains(m))
            {
                _multiblocks.Add(m);
                OnContainerUpdated();
            }
        }

        public void RegisterSmallMultiblock(Multiblock mb, Vector3 pos)
        {
            if (_smallMultiblocks.ContainsKey(pos))
            {
                Object.Destroy(_smallMultiblocks[pos].gameObject);
            }
            _smallMultiblocks[pos] = mb;
        }
        public GameObject GetMultiblocksAt(int x, int y, int z)
        {
            if (!_smallMultiblocks.ContainsKey(new Vector3(x, y, z)))
            {
                return null;
            }
            return _smallMultiblocks[new Vector3(x, y, z)].gameObject;
        }

        public void MineVoxel(int x, int y, int z, Inventory inventory)
        {
            var type = GetVoxelType(x, y, z);
            if (type.Drops != null)
            {
                foreach (var drop in type.Drops)
                {
                    var item = ObjectManager.GetObjectType(drop.Item);
                    var notStored = inventory.InsertItems(item, drop.Amount);
                    for (var i = 0; i < notStored; i++)
                    {
                        ObjectManager.DropObject(new Vector3(x, y, z) + Position, item);
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
}
