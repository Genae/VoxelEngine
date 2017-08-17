using Assets.Scripts.Logic;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class ToolBelt : UIView
    {
	    public void ButtonMine(){
		    //0 = mineTool atm
		    var go = GameObject.Find ("World").GetComponent<MouseController>();
		    go.SelectedTool = go.Tools [0];
	    }

	    public void ButtonBuild(){
		    var go = GameObject.Find ("World").GetComponent<MouseController>();
		    go.SelectedTool = go.Tools [1];
	    }

        public void ButtonFarm()
        {
            var go = GameObject.Find("World").GetComponent<MouseController>();
            go.SelectedTool = go.Tools[2];
        }

		public void ButtonMouseOver()
		{
			var go = GameObject.Find("World").GetComponent<MouseController>();
			go.SelectedTool = go.Tools[3];
        }
        public void ButtonAbort()
        {
            var go = GameObject.Find("World").GetComponent<MouseController>();
            go.SelectedTool = go.Tools[4];
        }
        public void ButtonPlaceObject()
        {
            var go = GameObject.Find("World").GetComponent<MouseController>();
            go.SelectedTool = go.Tools[5];
        }
    }

}