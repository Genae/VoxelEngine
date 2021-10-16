using System.Collections.Generic;
using System.Linq;
using ControlInputs;
using EngineLayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ToolBelt: MonoBehaviour
	{
	    public List<ToolConfig> MainGroup;
	    public ToolConfig SelectedItemMain;
	    public ToolConfig SelectedItemSecondary;
	    public Dictionary<string, ToolConfig[]> AllSecondaryGroups = new Dictionary<string, ToolConfig[]>();
	    public int xOffset;
	    
	    public GameObject ButtonPrefab;
	    public GameObject PanelPrefab;
	    public GameObject MainPanel;
	    public Dictionary<string,GameObject> Panels = new Dictionary<string, GameObject>();

	    private MouseController _controller;

        void Awake()
        {
            _controller = Object.FindObjectOfType<MouseController>();
            var configs = ConfigImporter.GetAllConfigs<ToolConfig[]>("Configs/Tools");
            MainGroup = new List<ToolConfig>();
            foreach (var config in configs)
            {
	            MainPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100*config.Length);
                foreach (var toolConfig in config)
                {
                    MainGroup.Add(toolConfig);
                    var b = Instantiate(ButtonPrefab, MainPanel.transform);
                    var tb = b.AddComponent<ToolButton>();
                    tb.Init(toolConfig, this);
                    if (toolConfig.Children != null && toolConfig.Children.Length > 0)
                    {
	                    Panels.Add(toolConfig.Name, Instantiate(PanelPrefab, transform));
	                    Panels[toolConfig.Name].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100*toolConfig.Children.Length);
	                    Panels[toolConfig.Name].SetActive(false);
                        AllSecondaryGroups[toolConfig.Name] = toolConfig.Children;
                        foreach (var toolConfigChild in toolConfig.Children)
                        {
	                        b = Instantiate(ButtonPrefab, Panels[toolConfig.Name].transform);
	                        tb = b.AddComponent<ToolButton>();
	                        tb.Init(toolConfigChild, this);
                        }
                    }
                }
                
            }
        }

	    public void SelectedMain(ToolConfig main)
	    {
		    SelectedItemMain = main;
	        
	        xOffset = (int) ((MainGroup.IndexOf(SelectedItemMain) + 0.5f - MainGroup.Count / 2f) * 100);
	        foreach (var panel in Panels)
	        {
		        panel.Value.gameObject.SetActive(false);
		    }
	        Panels[SelectedItemMain.Name].SetActive(true);

            _controller.SelectedTool = null;

	    }
	    public void SelectedSecondary(ToolConfig sec)
	    {
		    SelectedItemSecondary = sec;
	        _controller.SelectTool(SelectedItemSecondary.Tool);
        }
    }

	public class ToolButton: MonoBehaviour
	{
		private ToolConfig _config;
		private ToolBelt _parent;

		public void Init(ToolConfig config, ToolBelt parent)
		{
			_config = config;
			_parent = parent;
			transform.GetComponentInChildren<Text>().text = _config.Name;
			if(config.Children?.Any()??false)
				GetComponent<Button>().onClick.AddListener(MainSelected);
			else
			{
				GetComponent<Button>().onClick.AddListener(SecondarySelected);
			}
		}

		private void SecondarySelected()
		{
			_parent.SelectedSecondary(_config);
		}

		private void MainSelected()
		{
			_parent.SelectedMain(_config);
		}
	}

}