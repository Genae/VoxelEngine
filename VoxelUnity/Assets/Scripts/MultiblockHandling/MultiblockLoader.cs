using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts.Data.Multiblock;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Map;


namespace Assets.Scripts.MultiblockImporter
{
    public static class MultiblockLoader
    {

        public static void LoadMultiblock(string filename)
        {
            //load vdata list from text file
            var list = LoadVDataListFromFile(@Application.dataPath + "/Imported/", filename);

            //creating multiblock with vdata list
            var m = CreateMultiblock(list);
            m.transform.localScale = Vector3.one / 100; //10 is fraction value of importer, not very pretty atm
            m.transform.position = new Vector3(0, 0, 0);
        }

        public static List<VData> LoadVDataListFromFile(string path, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<VData>));
            StreamReader reader = new StreamReader(path + filename);
            return (List<VData>)serializer.Deserialize(reader);
        }

        public static Multiblock CreateMultiblock(List<VData> list)
        {
            var dict = new Dictionary<VoxelMaterial, List<Vector3>>();

            foreach (var data in list)
            {
                var color = Map.Instance.MaterialRegistry.GetColorIndex(data.Color);
                if (!dict.ContainsKey(color))
                    dict.Add(color, new List<Vector3>());
                dict[color].Add(data.VPos);
            }

            return Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), dict, "flowerpower");
        }

    }

}
