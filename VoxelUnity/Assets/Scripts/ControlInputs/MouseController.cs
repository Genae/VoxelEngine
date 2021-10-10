using System.Collections.Generic;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.GameLogicLayer.Tools;
using UnityEngine;

namespace Assets.Scripts.ControlInputs
{
    public class MouseController : MonoBehaviour
    {
        public Material PreviewMaterial;
        public Dictionary<string, Tool> Tools;
        private Tool _selectedTool;

        public Tool SelectedTool
        {
            get { return _selectedTool; }
            set
            {
                if(_selectedTool != null)
					_selectedTool.gameObject.SetActive(false);
                _selectedTool = value;
                if(value != null)
                    _selectedTool.gameObject.SetActive(true);
            }
        }

        public void SelectTool<T>()
        {
            SelectTool(typeof(T).FullName);
        }

        public void SelectTool(string tool)
        {
            SelectedTool = Tools[tool];
        }

        public void Awake()
        {
            Tools = new Dictionary<string, Tool>();
            var configs = ConfigImporter.GetAllConfigs<ToolConfig[]>("Configs/Tools");
            var parent = new GameObject("Tools");
            parent.transform.parent = transform;
            foreach (var config in configs)
            {
                foreach (var toolConfig in config)
                {
                    AddTools(toolConfig, parent.transform);
                }
            }
            SelectTool<AddBlocksTool>();
        }

        private void AddTools(ToolConfig toolConfig, Transform parent)
        {
            if (toolConfig.Children == null || toolConfig.Children.Length == 0)
            {
                var go = new GameObject(toolConfig.Name);
                go.AddComponent(System.Type.GetType(toolConfig.Tool));
                go.transform.parent = parent;
                Tools[toolConfig.Tool] = go.GetComponent<Tool>();
            }
            else
            {
                foreach (var toolConfigChild in toolConfig.Children)
                {
                    AddTools(toolConfigChild, parent);
                }
            }
        }
    }

    public class ToolConfig
    {
        public string Name;
        public ToolConfig[] Children;
        public string Tool;
    }
}
