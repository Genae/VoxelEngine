using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Data.Importer
{
    public class ConfigImporter
    {
        public static List<T> GetConfig<T>(string path)
        {
            List<T> list = new List<T>();
            var files = Resources.LoadAll<TextAsset>(path);
            foreach (var file in files)
            {
                list.Add(JsonConvert.DeserializeObject<T>(file.text));
            }
            return list;
        }
    }
}

