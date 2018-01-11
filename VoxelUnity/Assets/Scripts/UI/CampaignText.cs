using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class CampaignText : UIView
	{
        public _bool Visible;
        public _string Text;
        public static CampaignText Instance;

        void Awake()
        {
            Instance = this;
        }

	    void Start()
	    {
            Visible.Value = true;
        }
        
	    public void Close()
	    {
	        Visible.Value = false;
            Debug.Log("close");
        }
	    public void Open()
	    {
	        Visible.Value = true;
	    }
        
    }
}