using System.Collections;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Map : MonoBehaviour
    {
        public MapData MapData;
        public static Map Instance;
        public CameraController CameraController;
        public VoxelGraph AStarNetwork;
        public bool IsDoneGenerating;
        public bool GenerateMap; 

        public void Awake()
        {
            Instance = this;
        }
        
        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (!IsDoneGenerating)
                return;
            foreach (var chunkData in MapData.Chunks)
            {
                if (chunkData == null)
                    continue;
                chunkData.CheckDirtyVoxels();
            }
        }
        
        // ReSharper disable once UnusedMember.Local
        public IEnumerator CreateMap(BiomeConfiguration biomeConfig, GameLoader loader)
        {
            if (GenerateMap)
            {
                loader.SetStatus("Calculating Heightmap", 0.03f);
                var hmg = new HeightmapGenerator();
                yield return hmg.CreateHeightMap(129, 129, 42);
                loader.SetStatus("Building Map", 0.1f);
                MapData = new MapData(hmg.Values.GetLength(0) / Chunk.ChunkSize, 100 / Chunk.ChunkSize, 2f);
                AStarNetwork = new VoxelGraph();
                yield return MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100);
                yield return null;
                //RemoveTerrainNotOfType(new[] { MaterialRegistry.Instance.GetMaterialFromName("Iron"), MaterialRegistry.Instance.GetMaterialFromName("Gold"), MaterialRegistry.Instance.GetMaterialFromName("Copper"), MaterialRegistry.Instance.GetMaterialFromName("Coal") });
                //TestAStar();
            } else
            {
                MapData = new MapData(129 / Chunk.ChunkSize, 300 / Chunk.ChunkSize, 2f);
            }
            IsDoneGenerating = true;
            VoxelContainer.EnableDraw = true;
        }

        public ChunkData CreateChunk(int x, int y, int z)
        {
            MapData.Chunks[x, y, z] = new ChunkData(new Vector3(x, y, z) * Chunk.ChunkSize);
            Chunk.CreateChunk(x, y, z, this);
            return MapData.Chunks[x, y, z];
        }
        
        #region Tests
        private void RemoveTerrainNotOfType(VoxelMaterial[] types)
        {
            var air = MaterialRegistry.Instance.GetMaterialFromName("Air");
            for (var x = 0; x < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1) * Chunk.ChunkSize; y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; z++)
                    {
                        var mat = World.At(x, y, z).GetMaterial();
                        if (!types.Contains(mat))
                            World.At(x, y, z).SetVoxel(air);
                    }
                }
            }
        }
        #endregion

        public bool IsInBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < MapData.Chunks.GetLength(0) * Chunk.ChunkSize && y < MapData.Chunks.GetLength(1) * Chunk.ChunkSize && z < MapData.Chunks.GetLength(2) * Chunk.ChunkSize;
        }
    }

    public class BiomeConfiguration
    {
        public string Name;
        public ResourceConfiguration[] OreConfiguration;
        public AmbientPlantConfiguration[] AmbientPlants;
    }

    public class AmbientPlantConfiguration
    {
        public string Name;
        public int Amount;
    }

    public class ResourceConfiguration
    {
        public string ResourceType;
        public int MinAmount = 5;
        public int MaxAmount = 10;
        public float MinVeinLength = 30;
        public float VeinRadius = 2;
        public float Frequency = 7;
        
        private VoxelMaterial _material;
        public VoxelMaterial Material
        { 
            get { return _material ?? (_material = MaterialRegistry.Instance.GetMaterialFromName(ResourceType)); }
            set { _material = value; }
        }
    }
}