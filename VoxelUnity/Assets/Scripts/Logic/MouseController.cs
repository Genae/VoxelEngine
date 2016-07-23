using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class MouseController : MonoBehaviour {

        public Tool[] Tools = new Tool[0];
        private Tool _selectedTool;

        public Tool SelectedTool
        {
            get { return _selectedTool; }
            set
            {
				_selectedTool.gameObject.SetActive(false);
                _selectedTool = value;
                _selectedTool.gameObject.SetActive(true);
            }
        }

        public void Start()
        {
            SelectedTool = Tools[0];
        }
    }
}
