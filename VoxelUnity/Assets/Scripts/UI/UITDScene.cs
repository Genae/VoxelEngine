using System;
using System.Linq;
using MarkLight;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.GameLogicLayerTD;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UITDScene : View
    {
        public static UITDScene Instance;
        public _string ErrorText;
        public _bool Visible;
        public _bool Running;
        private float _timeVisible = 2;
        public float Cooldown;

        void Awake()
        {
            Instance = this;
        }

        public void ClickStart()
        {
            if(Running.Value)
                return;
            if (!CheckMannaz())
                return;
            if (!CheckUnlocked())
                return;
            if (!CheckMaxCount())
                return;
            var tdMap = Map.Instance.gameObject.AddComponent<TDMap>();
            tdMap.BuildMap();
        }

        private bool CheckMaxCount()
        {
            if (RuneRegistry.Runes.Count > 15)
            {
                ErrorText.Value = "Please do not place more than 15 runes.";
                Cooldown = _timeVisible;
                return false;
            }
            return true;
        }

        private bool CheckMannaz()
        {
            if (RuneRegistry.Runes.OfType<Mannaz>().Count() != 1)
            {
                ErrorText.Value = "Please place exactly 1 mannaz rune";
                Cooldown = _timeVisible;
                return false;
            }
            if (CampaignManager.Instance.CurrentLevel == 0)
                return true;
            var pos = RuneRegistry.Runes.OfType<Mannaz>().First().transform.position;
            var mapInfo = CampaignManager.Instance.GetMapInfo();
            var size = TDMap.GetSize(mapInfo.Village);
            var village = mapInfo.GetVillagePos(size);
            if (Vector3.Distance(pos, village) > 10)
            {
                ErrorText.Value = "Please the Mannaz rune in the correct position.";
                Cooldown = _timeVisible;
                return false;
            }
            return true;
        }

        private bool CheckUnlocked()
        {
            var unlocked = CampaignManager.Instance.UnlockedRunes;
            var placedRune = RuneRegistry.GetPlacedRuned();
            foreach (var pr in placedRune)
            {
                if (!unlocked.ContainsKey(pr.Key) || unlocked[pr.Key] < pr.Value)
                {
                    ErrorText.Value = "You have placed more runes of type " + pr.Key + " than you unlocked.(" + pr.Value + " of " + (unlocked.ContainsKey(pr.Key) ? unlocked[pr.Key] : 0) + ")";
                    Cooldown = _timeVisible;
                    return false;
                }
            }
            return true;
        }

        public void ClickClear()
        {
            TDMap.Instance.Clear();
        }

        void Update()
        {
            Cooldown -= Time.deltaTime;
            Visible.Value = Cooldown > 0;
        }
    }
}
