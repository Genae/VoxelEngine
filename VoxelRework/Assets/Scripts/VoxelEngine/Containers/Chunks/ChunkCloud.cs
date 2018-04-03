using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.VoxelEngine.Materials;
using Assets.Scripts.VoxelEngine.Renderers;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Containers.Chunks
{
    public class FluidChunkCloud : ChunkCloud
    {
        public FluidChunkCloud(MaterialCollection materialCollection, Transform map) : base(materialCollection, map)
        {}

        public FluidType GetFluidType(Vector3Int pos)
        {
            var t = base.GetVoxelType(pos);
            var h = (ushort)(8-(t >> 13));
            return new FluidType((ushort)((ushort)(t << 3) >> 3), h);
        }

        public ushort GetFluidHeight(Vector3Int pos)
        {
            var t = base.GetVoxelType(pos);
            return (ushort)(8 - (t >> 13));
        }

        public override ushort GetVoxelType(Vector3Int pos)
        {
            var t = base.GetVoxelType(pos);
            return (ushort)((ushort)(t << 3) >> 3);
        }

        public void SetVoxel(LoadedVoxelMaterial material, Vector3Int pos, ushort height)
        {
            SetVoxel(material.Id, pos, height);
        }

        public void SetVoxel(string material, Vector3Int pos, ushort height)
        {
            SetVoxel(MaterialCollection.GetId(material), pos, height);
        }

        public void SetVoxel(ushort material, Vector3Int pos, ushort height)
        {
            var h = (ushort)(8 - height);
            material += (ushort)(h << 13);
            SetVoxel(material, pos);
        }


        protected override string ChunkName()
        {
            return "FluidChunk";
        }
    }

    public class FluidType
    {
        public ushort Type;
        public ushort Height;

        public FluidType(ushort type, ushort height)
        {
            Type = type;
            Height = (ushort)(type == 0 ? 0 : height);
        }
    }

    public class ChunkCloud
    {
        protected readonly MaterialCollection MaterialCollection;
        private readonly Grid3D<Chunk> _chunks;
        private readonly Grid3D<MeshBuilder> _chunksMeshes;
        private readonly Transform _map;
        private int _slice = 100;

        //batch
        private bool _batchMode;
        private List<Vector3Int> _batchedChunks = new List<Vector3Int>();
        
        private object Lock = new object();

        public ChunkCloud(MaterialCollection materialCollection, Transform map)
        {
            MaterialCollection = materialCollection;
            _map = map;
            _chunks = new Grid3D<Chunk>();
            _chunksMeshes = new Grid3D<MeshBuilder>();
        }

        /*public int? GetTopVoxel(int x, int z, int maxy, int miny)
        {
            var cx = (x + (x < 0 ? 1 : 0)) / ChunkDataSettings.XSize - (x < 0 ? 1 : 0);
            var cyMax = (maxy + (maxy < 0 ? 1 : 0)) / ChunkDataSettings.YSize - (maxy < 0 ? 1 : 0);
            var cyMin = (miny + (miny < 0 ? 1 : 0)) / ChunkDataSettings.YSize - (miny < 0 ? 1 : 0);
            var cz = (z + (z < 0 ? 1 : 0)) / ChunkDataSettings.ZSize - (z < 0 ? 1 : 0);

            for (var cy = cyMax; cy >= cyMin; cy--)
            {
                if (_chunks[cx, cy, cz] == null)
                    continue;
                var top = _chunks[cx, cy, cz].GetTopVoxel(x%ChunkDataSettings.XSize, z%ChunkDataSettings.ZSize);
                if (top != null)
                    return top;
            }
            return null;
        }*/

        public virtual ushort GetVoxelType(Vector3Int pos)
        {
            var cx = (pos.x + (pos.x < 0 ? 1 : 0)) / ChunkDataSettings.XSize - (pos.x < 0 ? 1 : 0);
            var cy = (pos.y + (pos.y < 0 ? 1 : 0)) / ChunkDataSettings.YSize - (pos.y < 0 ? 1 : 0);
            var cz = (pos.z + (pos.z < 0 ? 1 : 0)) / ChunkDataSettings.ZSize - (pos.z < 0 ? 1 : 0);
            if (_chunks[cx, cy, cz] == null)
                return 0;
            var p = new Vector3Int(Mod(pos.x, ChunkDataSettings.XSize), Mod(pos.y, ChunkDataSettings.YSize), Mod(pos.z, ChunkDataSettings.ZSize));
            return _chunks[cx, cy, cz].GetVoxelData(p);
        }

        public void SetVoxel(LoadedVoxelMaterial material, Vector3Int pos)
        {
            SetVoxel(material.Id, pos);
        }
        public void SetVoxel(string material, Vector3Int pos)
        {
            SetVoxel(MaterialCollection.GetId(material), pos);
        }
        public void SetVoxel(ushort material, Vector3Int pos)
        {
            var cx = (pos.x + (pos.x < 0 ? 1 : 0)) / ChunkDataSettings.XSize - (pos.x < 0 ? 1 : 0);
            var cy = (pos.y + (pos.y < 0 ? 1 : 0)) / ChunkDataSettings.YSize - (pos.y < 0 ? 1 : 0);
            var cz = (pos.z + (pos.z < 0 ? 1 : 0)) / ChunkDataSettings.ZSize - (pos.z < 0 ? 1 : 0);
            var chunk = _chunks[cx, cy, cz] ?? _chunks.Init(cx, cy, cz, new Chunk());
            var p = new Vector3Int(Mod(pos.x, ChunkDataSettings.XSize), Mod(pos.y, ChunkDataSettings.YSize), Mod(pos.z, ChunkDataSettings.ZSize));
            var neighbours = chunk.SetVoxelData(p, material, MaterialCollection);
            if (_batchMode)
            {
                var cp = new Vector3Int(cx, cy, cz);
                if (!chunk.NeedsUpdate)
                {
                    chunk.NeedsUpdate = true;
                    lock (Lock)
                    {
                        _batchedChunks.Add(cp);
                    }
                }
            }
            else
            {
                var mySlice = _slice - cy * ChunkDataSettings.YSize;
                GetMeshBuilder(cx, cy, cz).BuildMeshAndApply(MaterialCollection, GetNeighbours(cx, cy, cz), chunk, mySlice, true);
            }
            foreach (var neighbour in neighbours)
            {
                var nPos = GetNeighbourPos(cx, cy, cz, neighbour);
                if (_chunksMeshes[nPos.x, nPos.y, nPos.z] != null)
                {
                    if (_batchMode)
                    {
                        if (!_chunks[nPos.x, nPos.y, nPos.z].NeedsUpdate)
                        {
                            _chunks[nPos.x, nPos.y, nPos.z].NeedsUpdate = true;
                            lock (Lock)
                            {
                                _batchedChunks.Add(nPos);
                            }
                        }
                    }
                    else
                    {
                        var mySlice = _slice - nPos.y * ChunkDataSettings.YSize;
                        GetMeshBuilder(nPos.x, nPos.y, nPos.z).BuildMeshAndApply(MaterialCollection, GetNeighbours(nPos.x, nPos.y, nPos.z), _chunks[nPos.x, nPos.y, nPos.z], mySlice, true);
                    }
                }
            }
        }

        private MeshBuilder GetMeshBuilder(int x, int y, int z)
        {
            if (_chunksMeshes[x, y, z] == null)
            {
                _chunksMeshes.Init(x, y, z, BuildChunkMeshBuilder(x, y, z));
            }
            return _chunksMeshes[x, y, z];
        }

        protected virtual string ChunkName()
        {
            return "Chunk";
        }

        private MeshBuilder BuildChunkMeshBuilder(int cx, int cy, int cz)
        {
            var go = new GameObject($"{ChunkName()} [{cx}, {cy}, {cz}]");
            go.transform.parent = _map;
            go.transform.localPosition = new Vector3(cx * ChunkDataSettings.XSize, cy * ChunkDataSettings.YSize,
                cz * ChunkDataSettings.ZSize);
            var mb = go.AddComponent<MeshBuilder>();
            mb.Init();
            return mb;
        }

        public static int Mod(int num, ushort mod)
        {
            return (num % mod + mod) % mod;
        }

        private Dictionary<ChunkSide, Chunk> GetNeighbours(int cx, int cy, int cz)
        {
            return new Dictionary<ChunkSide, Chunk>
            {
                {ChunkSide.Px, _chunks[cx + 1, cy, cz] },
                {ChunkSide.Nx, _chunks[cx - 1, cy, cz] },
                {ChunkSide.Py, _chunks[cx, cy + 1, cz] },
                {ChunkSide.Ny, _chunks[cx, cy - 1, cz] },
                {ChunkSide.Pz, _chunks[cx, cy, cz + 1] },
                {ChunkSide.Nz, _chunks[cx, cy, cz - 1] }
            };
        }

        private Vector3Int GetNeighbourPos(int cx, int cy, int cz, ChunkSide side)
        {
            switch (side)
            {
                case ChunkSide.Px:
                    return new Vector3Int(cx + 1, cy, cz);
                case ChunkSide.Nx:
                    return new Vector3Int(cx - 1, cy, cz);
                case ChunkSide.Py:
                    return new Vector3Int(cx, cy + 1, cz);
                case ChunkSide.Ny:
                    return new Vector3Int(cx, cy - 1, cz);
                case ChunkSide.Pz:
                    return new Vector3Int(cx, cy, cz + 1);
                case ChunkSide.Nz:
                    return new Vector3Int(cx, cy, cz - 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        public void StartBatch()
        {
            _batchMode = true;
        }

        public void FinishBatch()
        {
            _batchMode = false;
            var tasks = new Task[_batchedChunks.Count];
            var i = 0;
            foreach (var batchedChunk in _batchedChunks)
            {
                var mySlice = _slice - batchedChunk.y * ChunkDataSettings.YSize;
                
                tasks[i++] = GetMeshBuilder(batchedChunk.x, batchedChunk.y, batchedChunk.z).BuildMesh(MaterialCollection, GetNeighbours(batchedChunk.x, batchedChunk.y, batchedChunk.z), _chunks[batchedChunk.x, batchedChunk.y, batchedChunk.z], mySlice, true); ; 
                _chunks[batchedChunk.x, batchedChunk.y, batchedChunk.z].NeedsUpdate = false;
            }
            Task.WaitAll(tasks);
            foreach (var batchedChunk in _batchedChunks)
            {
                var mySlice = _slice - batchedChunk.y * ChunkDataSettings.YSize;
                GetMeshBuilder(batchedChunk.x, batchedChunk.y, batchedChunk.z).ApplyMeshData(MaterialCollection, mySlice, true);
            }
            _batchedChunks.Clear();
        }

        public void SetSlice(int slice)
        {
            _slice = slice;
            MaterialCollection.SetSlice(slice);
            foreach (var mesh in _chunksMeshes)
            {
                var mySlice = slice - mesh.Key.y * ChunkDataSettings.YSize;
                mesh.Value.BuildMesh(MaterialCollection, GetNeighbours(mesh.Key.x, mesh.Key.y, mesh.Key.z), _chunks[mesh.Key.x, mesh.Key.y, mesh.Key.z], mySlice, false);
            }
            foreach (var mesh in _chunksMeshes)
            {
                var mySlice = slice - mesh.Key.y * ChunkDataSettings.YSize;
                mesh.Value.ApplyMeshData(MaterialCollection, mySlice, false);
            }

        }
    }
}
