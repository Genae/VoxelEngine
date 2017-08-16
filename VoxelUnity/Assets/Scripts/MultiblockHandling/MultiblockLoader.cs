using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MultiblockHandling
{
    public class MultiblockLoader
    {
        Dictionary<int, Multiblock> _loadedObjects = new Dictionary<int, Multiblock>();
        private static MultiblockLoader _instance;
        private Transform _loadedObjectsCache;

        protected static MultiblockLoader Instance
        {
            get { return _instance ?? (_instance = new MultiblockLoader()); }
            set { _instance = value; }
        }

        public static Multiblock LoadMultiblock(string filename, Vector3 position = default(Vector3), Transform parent = null, float wind = 0f)
        {
            if (Instance._loadedObjectsCache == null)
            {
                Instance._loadedObjectsCache = new GameObject("LoadedObjectsCache").transform;
                Instance._loadedObjectsCache.gameObject.SetActive(false);
            }
            if (!Instance._loadedObjects.ContainsKey(filename.GetHashCode()))
            {
                //load vdata list from text file
                var list = Instance.LoadVDataListFromFile("Imported/", filename);

                //creating multiblock with vdata list
                var m = Instance.CreateMultiblock(list, filename.Split('/').Last());
                m.transform.localScale = Vector3.one / 10; //10 is fraction value of importer, not very pretty atm
                m.transform.position = new Vector3(0, 0, 0);
                m.transform.parent = Instance._loadedObjectsCache;
                Instance._loadedObjects[filename.GetHashCode()] = m;
                if(wind > 0)
                    m.EnableWind(wind);
            }
            var obj = Object.Instantiate(Instance._loadedObjects[filename.GetHashCode()].gameObject).GetComponent<Multiblock>();
            obj.transform.position = position;
            obj.transform.parent = parent;
            return obj;
        }

        public static void CleanupCache()
        {
            Instance._loadedObjects.Clear();
            Object.DestroyImmediate(Instance._loadedObjectsCache.gameObject);
            Instance._loadedObjectsCache = null;
        }
        
        private List<VData> LoadVDataListFromFile(string path, string filename)
        {
            var files = Resources.Load<TextAsset>(path + filename);
            string jsonstring = files.text;
            return JsonConvert.DeserializeObject<VData[]>(jsonstring).ToList();
        }

        private Multiblock CreateMultiblock(List<VData> list, string name)
        {
            var dict = new Dictionary<VoxelMaterial, List<Vector3>>();

            foreach (var data in list)
            {
                var color = MaterialRegistry.Instance.GetColorIndex(new Color(data.Color.X/256f, data.Color.Y/256f, data.Color.Z/256f));
                if (!dict.ContainsKey(color))
                    dict.Add(color, new List<Vector3>());
                dict[color].Add(new Vector3(data.VPos.X, data.VPos.Y, data.VPos.Z));
            }

            return Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), dict, name);
        }
    }

}
