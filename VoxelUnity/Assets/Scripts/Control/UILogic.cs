using UnityEngine;
using System.Collections;
using Assets.Scripts.Logic;

namespace Assets.Scripts.Control
{
	public class UILogic : MonoBehaviour {

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
