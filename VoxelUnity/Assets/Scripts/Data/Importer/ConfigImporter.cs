using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Assets.Scripts.Data.Importer
{
    public class ConfigImporter
    {
        public static List<T> GetConfig<T>(string path)
        {
            List<T> list = new List<T>();
            foreach (var file in Directory.GetFiles(path).Where(f => f.EndsWith(".json")))
            {
                var json = File.ReadAllText(file);
                list.Add(JsonConvert.DeserializeObject<T>(json));
            }
            return list;
        }
    }
}

