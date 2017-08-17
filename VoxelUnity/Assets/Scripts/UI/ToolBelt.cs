using Assets.Scripts.Logic;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class ToolBelt : UIView
    {
	    public void ButtonMine(){
		    //0 = mineTool atm
		    var go = FindObjectOfType<MouseController>();
	        go.SelectTool("Assets.Scripts.Logic.Tools.DeleteTool");
	    }

	    public void ButtonBuild(){
		    var go = FindObjectOfType<MouseController>();
	        go.SelectTool("Assets.Scripts.Logic.Tools.AddBlocksTool");
        }

        public void ButtonFarm()
        {
            var go = FindObjectOfType<MouseController>();
            go.SelectTool("Assets.Scripts.Logic.Tools.FarmTool");
        }

		public void ButtonMouseOver()
		{
			var go = FindObjectOfType<MouseController>();
		    go.SelectTool("Assets.Scripts.Logic.Tools.MouseoverTool");
        }
        public void ButtonAbort()
        {
            var go = FindObjectOfType<MouseController>();
            go.SelectTool("Assets.Scripts.Logic.Tools.AbortJobsTool");
        }
        public void ButtonPlaceObject()
        {
            var go = FindObjectOfType<MouseController>();
            go.SelectTool("Assets.Scripts.Logic.Tools.PlaceObjectTool");
        }
    }

}