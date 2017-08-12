using System.Collections;
using System.Linq;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    public class GameLoader : MonoBehaviour
    {
        public static bool GameLoaded = false;
        Text loadingStatus;
        Image loadingProgress;

        void Awake()
        {
            MainThread.Instantiate();
            StartCoroutine(LoadGame());
        }
        
        IEnumerator LoadGame()
        {
            Time.timeScale = 0;
            //Init Loading Screen
            var loadingScreen = GameObject.Find("LoadingScreenImage");
            loadingStatus = loadingScreen.transform.Find("Status").GetComponent<Text>();
            loadingProgress = loadingScreen.transform.Find("ProgressFG").GetComponent<Image>();
            loadingScreen.SetActive(true);

            //Load Materials
            SetStatus("Loading Materials", 0.01f);
            MaterialRegistry.Instance.Preload();
            yield return null;

            //Load Map
            SetStatus("Loading Map", 0.02f);
            var biomeConfig = ConfigImporter.GetConfig<BiomeConfiguration>("Biomes").First();
            yield return Map.Map.Instance.CreateMap(biomeConfig, this);
            loadingProgress.fillAmount = 0.8f;

            //Trees
            SetStatus("Loading Trees", 0.8f);
            var treeManager = new TreeManager();
            yield return treeManager.GenerateTrees((int)(Map.Map.Instance.MapData.Chunks.GetLength(0) * Map.Map.Instance.MapData.Chunks.GetLength(0) * 0.3f), Map.Map.Instance.MapData, this);

            //Ressources
            SetStatus("Loading Ressources", 0.99f);
            var resourceManager = new ResourceManager();
            resourceManager.SpawnAllResources(Map.Map.Instance.MapData, biomeConfig.OreConfiguration);


            //Start Time Again
            Time.timeScale = 1;
            yield return null;
            GameLoaded = true;

            loadingScreen.SetActive(false);
            SetCameraValues();
        }

        public void SetStatus(string text, float progress)
        {
            loadingStatus.text = text;
            loadingProgress.fillAmount = progress;
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
