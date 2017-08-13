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
    }

}