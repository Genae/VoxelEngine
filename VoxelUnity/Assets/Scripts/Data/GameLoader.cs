using System.Collections;
using System.Linq;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class GameLoader : MonoBehaviour
    {
        public static bool GameLoaded = false;
        private LoadingScreen _loadingScreen;

        void Awake()
        {
            MainThread.Instantiate();
            StartCoroutine(LoadGame());
        }
        
        IEnumerator LoadGame()
        {
            Time.timeScale = 0;
            yield return null;
            _loadingScreen = FindObjectOfType<LoadingScreen>();
            _loadingScreen.IsVisible.Value = true;

            //Load Materials
            SetStatus("Loading Materials", 0.01f);
            yield return null;
            MaterialRegistry.Instance.Preload();
            yield return null;

            //Load Map
            SetStatus("Loading Map", 0.02f);
            yield return null;
            var biomeConfig = ConfigImporter.GetAllConfigs<BiomeConfiguration>("World/Biomes").First();
            yield return Map.Map.Instance.CreateMap(biomeConfig, this);

            //Trees
            SetStatus("Loading Tree of Life", 0.8f);
            yield return null;
            var treeManager = new TreeManager();
            treeManager.BuildTreeOfLife();
            SetStatus("Loading Trees", 0.85f);
            yield return null;
            yield return treeManager.GenerateTrees((int)(Map.Map.Instance.MapData.Chunks.GetLength(0) * Map.Map.Instance.MapData.Chunks.GetLength(0) * 0.3f), Map.Map.Instance.MapData, this);

            //Ressources
            SetStatus("Loading Ressources", 0.95f);
            yield return null;
            var resourceManager = new ResourceManager();
            resourceManager.SpawnAllResources(Map.Map.Instance.MapData, biomeConfig.OreConfiguration);
            yield return null;

            //Characters
            SetStatus("Loading Characters", 0.96f);
            yield return null;
            var gameController = FindObjectOfType<GameController>();
            for (int i = 0; i < 5; i++)
            {
                gameController.SpawnCharacter();
            }

            //AmbientPlants
            SetStatus("Loading Ambient Plants", 0.97f);
            yield return null;
            gameController.SpawnAmbientPlants(biomeConfig);

            //AddT1Nodes
            SetStatus("Finish Pathfinder", 0.98f);
            yield return null;
            Map.Map.Instance.AStarNetwork.AddTier1Nodes(20);

            SetStatus("Done", 1f);
            //Start Time Again
            Time.timeScale = 1;
            yield return null;
            GameLoaded = true;

            _loadingScreen.IsVisible.Value = false;
            SetCameraValues();
        }

        public void SetStatus(string text, float progress)
        {
            _loadingScreen.StatusText.Value = text;
            _loadingScreen.Progress.Value = progress;
        }


        private void SetCameraValues()
        {
            var cameraController = Map.Map.Instance.CameraController;
            var chunks = Map.Map.Instance.MapData.Chunks;
            if (cameraController == null) return;
            var mapSize = chunks.GetLength(0) * Chunk.ChunkSize;
            var mapHeight = chunks.GetLength(1) * Chunk.ChunkSize;
            cameraController.RightLimit = mapSize * 1.1f;
            cameraController.TopLimit = mapSize * 1.1f;
            cameraController.CameraMinHeight = mapHeight * 0.5f;
            cameraController.CameraMaxHeight = mapHeight * 1.5f;

            cameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);
            cameraController.RotateTo(55);
            cameraController.Eye.gameObject.transform.position = new Vector3(0, cameraController.CameraMaxHeight, 0);
        }
    }
}
