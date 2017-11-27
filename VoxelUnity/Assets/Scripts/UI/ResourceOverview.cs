using System.ComponentModel.Design.Serialization;
using Assets.Scripts.GameLogicLayerTD;
using MarkLight;
using MarkLight.Views.UI;

namespace Assets.Scripts.UI
{
    public class ResourceOverview : UIView
    {
        public static ResourceOverview Instance;
        public _int Gold;
        public _int Upkeep;
        public _int Lives;
        public _bool Lost;

        public ResourceOverview()
        {
        }

        void Start()
        {
            Gold.Value = 10;
            Upkeep.Value = 0;
            Lives.Value = 10;
            Instance = this;
        }

        void Update()
        {
            if (!Lost && Lives <= 0)
            {
                Lost.Value = true;
                foreach (var instanceTower in TDMap.Instance.Towers)
                {
                    instanceTower.Explode();
                }
                foreach (var instanceFarm in TDMap.Instance.Farms)
                {
                    instanceFarm.Explode();
                }
            }
        }
    }
}
