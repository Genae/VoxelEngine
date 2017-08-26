using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class GameSpeed : UIView
    {
        public void ClickNormal()
        {
            Time.timeScale = 1;
        }

        public void ClickFast()
	    {
	        Time.timeScale = 2;
	    }

	    public void ClickVeryFast()
        {
	        Time.timeScale = 5;
        }
        
        public void ClickSlow()
        {
            Time.timeScale = 0.3f;
        }
    }

}