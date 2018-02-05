using System.Linq;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.GameLogicLayerTD;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CampaignText : UIView
	{
        public _bool Visible;
        public _string Text;
	    public _string Text2;
	    public Sprite Image;
        public static CampaignText Instance;
	    private RuneDescription[] _runeDescriptions;

        void Awake()
        {
            _runeDescriptions = ConfigImporter.GetAllConfigs<RuneDescription[]>("Configs/RuneDescriptions").First();
            Instance = this;
        }

	    void Start()
	    {
            Visible.Value = true;
        }
        
	    public void Close()
	    {
	        Text2.Value = "";
	        Text.Value = "";
            if (CampaignManager.Instance.NewlyUnlocked.Count > 0)
	        {
	            var unlocked = CampaignManager.Instance.NewlyUnlocked[0];
	            CampaignManager.Instance.NewlyUnlocked.RemoveAt(0);
	            Text.Value = "<size=50>You unlocked " + unlocked + "!</size>";
	            var runeDescription = _runeDescriptions.First(r => r.ID.Equals(unlocked.ToLower()));
	            SetValue(() => Image, runeDescription.Image);
                Text2.Value = "Description: " + runeDescription.Usage;
	            return;
	        }
	        if (CampaignManager.Instance.UnlockedThisTime.Count > 0)
	        {
	            Text.Value = "<size=30>You unlocked the following Runes:</size>\n\n";
	            foreach (var utt in CampaignManager.Instance.UnlockedThisTime)
	            {
	                Text.Value += utt.Value + "x <b>" + utt.Key + "</b> (you now have " + CampaignManager.Instance.UnlockedRunes[utt.Key] + ")\n";
	            }
	            CampaignManager.Instance.UnlockedThisTime.Clear();
	            return;
	        }
            Visible.Value = false;
        }
	    public void Open()
	    {
	        Visible.Value = true;
	    }
        
    }
}