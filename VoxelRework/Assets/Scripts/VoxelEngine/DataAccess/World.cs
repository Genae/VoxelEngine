using System.Linq;
using Assets.Scripts.VoxelEngine.Containers;
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
        public FluidUpdaterCloud FluidUpdater;

        public World(MaterialCollection materialCollection)
        {
            _materialCollection = materialCollection;
            var go = new GameObject("map");
            SolidChunks = new ChunkCloud(materialCollection, go.transform);
            FluidChunks = new FluidChunkCloud(materialCollection, go.transform);
            FluidUpdater = go.AddComponent<FluidUpdaterCloud>();
            FluidUpdater.World = this;
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

        public void SetVoxel(LoadedVoxelMaterial mat, Vector3Int pos, ushort height = 8)
        {
            if (mat.Fluid)
            {
                if (height == 0)
                {
                    FluidChunks.SetVoxel(_materialCollection.GetById(0), pos);
                    FluidUpdater.SetVoxel(mat, height, pos);
                }
                else
                {
                    FluidChunks.SetVoxel(mat, pos, height);
                    FluidUpdater.SetVoxel(mat, height, pos);
                }
            }
            else
            {
                SolidChunks.SetVoxel(mat, pos);
            }
        }

        public LoadedVoxelMaterial GetVoxelMaterial(Vector3Int pos)
        {
            return _materialCollection.GetById(GetVoxelType(pos));
        }

        public ushort GetVoxelType(Vector3Int pos)
        {
            return SolidChunks.GetVoxelType(pos);
        }
        public FluidType GetFluidAtPos(Vector3Int pos)
        {
            return FluidChunks.GetFluidType(pos);
        }
    }

    public class FluidUpdaterCloud : MonoBehaviour
    {
        private readonly Grid3D<FluidUpdater> _chunks = new Grid3D<FluidUpdater>();
        public World World;

        public void SetVoxel(LoadedVoxelMaterial mat, ushort height, Vector3Int pos)
        {
            var cx = (pos.x + (pos.x < 0 ? 1 : 0)) / ChunkDataSettings.XSize - (pos.x < 0 ? 1 : 0);
            var cy = (pos.y + (pos.y < 0 ? 1 : 0)) / ChunkDataSettings.YSize - (pos.y < 0 ? 1 : 0);
            var cz = (pos.z + (pos.z < 0 ? 1 : 0)) / ChunkDataSettings.ZSize - (pos.z < 0 ? 1 : 0);
            var chunk = _chunks[cx, cy, cz] ?? _chunks.Init(cx, cy, cz, new FluidUpdater());
            chunk.SetVoxel(mat, height, pos);
        }

        void Update()
        {
            foreach (var chunk in _chunks.OrderBy(c => c.Key.y).ToArray())
            {
                chunk.Value.RunUpdate(Time.deltaTime, World);
            }
        }
    }

    public class FluidUpdater
    {
        private readonly Grid3D<Fluid> _fluids = new Grid3D<Fluid>();
        private float cooldown = 1;

        public void SetVoxel(LoadedVoxelMaterial mat, ushort height, Vector3Int pos)
        {
            if (mat.Id == 0 || height == 0)
            {
                _fluids.Remove(new Vector3Int(pos.x, pos.y, pos.z));
            }
            else
            {
                _fluids[pos.x, pos.y, pos.z] = new Fluid(mat, height);
            }
        }

        public void RunUpdate(float deltaTime, World world)
        {
            if ((cooldown -= deltaTime) <= 0)
            {
                cooldown = 0.1f;
                UpdateFluids(deltaTime, world);
            }
        }

        private void UpdateFluids(float deltaTime, World world)
        {
            world.StartBatch();
            Debug.Log("Update Fluids");
            foreach (var fluid in _fluids.OrderBy(c => c.Key.y).ToArray())
            {
                fluid.Value.RunUpdate(deltaTime, fluid.Key, world);
            }
            world.FinishBatch();
        }
    }

    internal class Fluid
    {
        public LoadedVoxelMaterial Mat;
        public ushort Height;

        public Fluid(LoadedVoxelMaterial mat, ushort height)
        {
            Mat = mat;
            Height = height;
        }

        public void RunUpdate(float deltaTime, Vector3Int worldPos, World world)
        {
            var below = new Vector3Int(worldPos.x, worldPos.y - 1, worldPos.z);
            if (world.GetVoxelType(below) == 0) // Gravity
            {
                var fluidBelow = world.GetFluidAtPos(below);
                if (fluidBelow.Height != 8)
                {
                    Gravity(worldPos, world, fluidBelow, below);
                }
                else
                {
                    Spread(worldPos, world);
                }
            }
            else
            {
                Spread(worldPos, world);
            }
        }

        private void Spread(Vector3Int worldPos, World world)
        {
            var px = new Vector3Int(worldPos.x + 1, worldPos.y, worldPos.z);
            var fpx = world.GetVoxelMaterial(px).Id == 0 ? world.GetFluidAtPos(px) : null;
            var nx = new Vector3Int(worldPos.x - 1, worldPos.y, worldPos.z);
            var fnx = world.GetVoxelMaterial(nx).Id == 0 ? world.GetFluidAtPos(nx) : null;
            var pz = new Vector3Int(worldPos.x, worldPos.y, worldPos.z + 1);
            var fpz = world.GetVoxelMaterial(pz).Id == 0 ? world.GetFluidAtPos(pz) : null;
            var nz = new Vector3Int(worldPos.x, worldPos.y, worldPos.z - 1);
            var fnz = world.GetVoxelMaterial(nz).Id == 0 ? world.GetFluidAtPos(nz) : null;

            var avg = 0;
            var count = 0;
            if (fpx != null && fpx.Height < Height)
            {
                avg += fpx.Height;
                count++;
            }
            if (fnx != null && fnx.Height < Height)
            {
                avg += fnx.Height;
                count++;
            }
            if (fpz != null && fpz.Height < Height)
            {
                avg += fpz.Height;
                count++;
            }
            if (fnz != null && fnz.Height < Height)
            {
                avg += fnz.Height;
                count++;
            }
            if (count == 0)
                return;
            avg = avg / count;

            var transfer = (Height - avg) / (count + 1);
            var transfered = 0;
            if (fpx != null && fpx.Height < Height)
            {
                world.SetVoxel(Mat, px, (ushort)(fpx.Height + transfer));
                transfered += transfer;
            }
            if (fnx != null && fnx.Height < Height)
            {
                world.SetVoxel(Mat, nx, (ushort)(fnx.Height + transfer));
                transfered += transfer;
            }
            if (fpz != null && fpz.Height < Height)
            {
                world.SetVoxel(Mat, pz, (ushort)(fpz.Height + transfer));
                transfered += transfer;
            }
            if (fnz != null && fnz.Height < Height)
            {
                world.SetVoxel(Mat, nz, (ushort)(fnz.Height + transfer));
                transfered += transfer;
            }
            if (transfered > 0)
            {
                world.SetVoxel(Mat, worldPos, (ushort)(Height - transfered));
            }

        }

        private void Gravity(Vector3Int worldPos, World world, FluidType fluidBelow, Vector3Int below)
        {
            var transfer = (ushort) Mathf.Max(8 - fluidBelow.Height, Height);
            world.SetVoxel(Mat, worldPos, (ushort) (Height - transfer));
            world.SetVoxel(Mat, below, (ushort) (fluidBelow.Height + transfer));
        }
    }
}
