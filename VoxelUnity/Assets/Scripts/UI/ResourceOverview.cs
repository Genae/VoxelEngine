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
        public int MaxGold = 500;

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
            if (Gold.Value > MaxGold)
            {
                Gold.Value = MaxGold;
            }
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
