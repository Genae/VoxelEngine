using System.Collections.Generic;
using EngineLayer;

namespace AccessLayer.Farming
{
    public class CropManager
    {
        private static CropManager _instance;
        public static CropManager Instance
        {
            get { return _instance ?? (_instance = new CropManager()); }
        }

        private readonly Dictionary<string, CropType> _crops = new Dictionary<string, CropType>();

        public static void LoadCrops()
        {
            var configs = ConfigImporter.GetAllConfigs<CropConfig>("World/Crops");
            foreach (var cropConfig in configs)
            {
                var cropType = new CropType(cropConfig.Name, cropConfig.StageTime);
                for (var i = 1; i <= cropConfig.StageCount; i++)
                {
                    cropType.GrowStages[i] = "Plants/Crops/" + cropConfig.Name + "/" + i;
                }
                Instance._crops[cropType.Name] = cropType;
            }

        }

        public CropType GetCropByName(string name)
        {
            if(_crops.Count == 0)
                LoadCrops();
            return _crops[name];
        }
    }

    public class CropConfig
    {
        public float StageTime;
        public string Name;
        public int StageCount;
    }
}
