using System.Collections.Generic;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Logic;
using MarkLight;
using MarkLight.Views.UI;

namespace Assets.Scripts.UI
{
	public class ToolBelt : UIView
	{
	    public ObservableList<ToolConfig> MainGroup;
	    public ObservableList<ToolConfig> SecondaryGroup = new ObservableList<ToolConfig>();
	    public Dictionary<string, ToolConfig[]> AllSecondaryGroups = new Dictionary<string, ToolConfig[]>();
	    public _int xOffset;

	    private MouseController _controller;

        void Awake()
        {
            _controller = FindObjectOfType<MouseController>();
            var configs = ConfigImporter.GetAllConfigs<ToolConfig[]>("Configs/Tools");
            MainGroup = new ObservableList<ToolConfig>();
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
	        var list = AllSecondaryGroups[MainGroup.SelectedItem.Name];

	        xOffset.Value = (int) ((MainGroup.SelectedIndex + 0.5f - MainGroup.Count / 2f) * 100);

	        SecondaryGroup.Clear();
            SecondaryGroup.AddRange(list);
            _controller.SelectedTool = null;

	    }
	    public void SelectedSecondary()
	    {
	        _controller.SelectTool(SecondaryGroup.SelectedItem.Tool);
        }
    }

}