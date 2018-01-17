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
        public _string ErrorText;
        public _bool Visible;
        private float _timeVisible = 2;
        public float Cooldown;

        public void ClickStart()
        {
            if (!CheckRaido())
                return;
            if (!CheckMannaz())
                return;
            if (!CheckUnlocked())
                return;
            var tdMap = Map.Instance.gameObject.AddComponent<TDMap>();
            tdMap.BuildMap();
        }

        private bool CheckRaido()
        {
            if (RuneRegistry.Runes.OfType<Raido>().Count() < 4)
            {
                ErrorText.Value = "Please place more raido runes";
                Cooldown = _timeVisible;
                return false;
            }
            return true;
        }

        private bool CheckMannaz()
        {
            if (CampaignManager.Instance.CurrentLevel == 0)
                return true;
            if (RuneRegistry.Runes.OfType<Mannaz>().Count() != 1)
            {
                ErrorText.Value = "Please place exactly 1 mannaz rune";
                Cooldown = _timeVisible;
                return false;
            }
            var pos = RuneRegistry.Runes.OfType<Mannaz>().First().transform.position;
            var mapInfo = CampaignManager.Instance.GetMapInfo();
            var size = TDMap.GetSize(RuneRegistry.Runes.OfType<Raido>().ToList());
            var village = mapInfo.GetVillagePos(size);
            if (Vector3.Distance(pos, village) > 5)
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
