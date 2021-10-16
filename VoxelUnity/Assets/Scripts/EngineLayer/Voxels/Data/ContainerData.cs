using System;
using System.Collections.Generic;
using AccessLayer.Material;
using Algorithms.Pathfinding.Utils;
using EngineLayer.Voxels.Containers;
using EngineLayer.Voxels.Material;
using UnityEngine;

namespace EngineLayer.Voxels.Data
{
    public class ContainerData
    {
        protected readonly List<Vector3> DirtyVoxels = new List<Vector3>();
        public readonly VoxelData[,,] Voxels;
        public readonly int Size;
        private readonly float Scale;
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
            ContainerUpdated?.Invoke();
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