using System.Collections.Generic;
using Assets.Scripts.ControlInputs;
using Assets.Scripts.EngineLayer;
using Delight;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class ToolBelt : UIView
	{
	    public List<ToolConfig> MainGroup;
	    public ToolConfig SelectedItemMain;
	    public List<ToolConfig> SecondaryGroup = new List<ToolConfig>();
	    public ToolConfig SelectedItemSecondary;
	    public Dictionary<string, ToolConfig[]> AllSecondaryGroups = new Dictionary<string, ToolConfig[]>();
	    public int xOffset;

	    private MouseController _controller;

        void Awake()
        {
            _controller = Object.FindObjectOfType<MouseController>();
            var configs = ConfigImporter.GetAllConfigs<ToolConfig[]>("Configs/Tools");
            MainGroup = new List<ToolConfig>();
            foreach (var config in configs)
            {
                foreach (var toolConfig in config)
                {
                    MainGroup.Add(toolConfig);
                    if (toolConfig.Children != null && toolConfig.Children.Length > 0)
                    {
                        AllSecondaryGroups[toolConfig.Name] = toolConfig.Children;
                    }
                }
                
            }
        }

	    public void SelectedMain()
	    {
	        var list = AllSecondaryGroups[SelectedItemMain.Name];

	        xOffset = (int) ((MainGroup.IndexOf(SelectedItemMain) + 0.5f - MainGroup.Count / 2f) * 100);

	        SecondaryGroup.Clear();
            SecondaryGroup.AddRange(list);
            _controller.SelectedTool = null;

	    }
	    public void SelectedSecondary()
	    {
	        _controller.SelectTool(SelectedItemSecondary.Tool);
        }
    }

}