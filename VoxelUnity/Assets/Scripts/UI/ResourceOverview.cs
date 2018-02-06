using System.ComponentModel.Design.Serialization;
using Assets.Scripts.GameLogicLayerTD;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using Vuforia.EditorClasses;

namespace Assets.Scripts.UI
{
    public class ResourceOverview : UIView
    {
        public static ResourceOverview Instance;
        public _int Gold;
        public _int Lives;
        public _bool Lost;
        public _bool Disabled;
        public bool Running;
        public int MaxGold = 500;

        public ResourceOverview()
        {
        }

        public void RetryLevel()
        {
            Lost.Value = false;
            Lives.Value = 10;
            CampaignManager.Instance.ResetLevel();
        }

        void Start()
        {
            Gold.Value = 10;
            Lives.Value = 10;
            Disabled.Value = false;
            Instance = this;
        }

        public void IncreaseDefense()
        {
            Gold.Value -= Lives;
            Lives.Value++;
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

            Disabled.Value = Running || Gold.Value < Lives.Value;
        }
    }
}
