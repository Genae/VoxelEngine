using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ControlInputs;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.GameLogicLayer.Tools;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class RuneOverview : UIView
	{
	    public static RuneOverview Instance;
        public _bool PlaceEnabled;
        public ObservableList<RuneDescription>[] AllRunes = new ObservableList<RuneDescription>[4];
	    public ObservableList<RuneDescription> Runes0, Runes1, Runes2, Runes3;
	    public ObservableList<int> Buttons = new ObservableList<int> {0, 1, 2};
	    public RuneDescription SelectedDescription;
	    public _bool Visible;
		public ViewSwitcher ContentViewSwitcher;
	    private PlaceRuneTool _prt;
	    private MouseController _mc;
	    public _int HeightScroll;

        void Awake()
        {
            Instance = this;
            var descriptions = ConfigImporter.GetAllConfigs<RuneDescription[]>("Configs/RuneDescriptions").First();
            for (int i = 0; i < descriptions.Length; i++)
            {
                if(AllRunes[i / 6] == null)
                    AllRunes[i / 6] = new ObservableList<RuneDescription>();
                AllRunes[i / 6].Add(descriptions[i]);
            }
            Runes0 = AllRunes[0];
            Runes1 = AllRunes[1];
            Runes2 = AllRunes[2];
            Runes3 = AllRunes[3];
            HeightScroll.Value = 5000;
        }

	    void Start()
	    {
	        _prt = FindObjectOfType<PlaceRuneTool>();
	        _mc = FindObjectOfType<MouseController>();
        }
        
        public void BuildRune() { }

	    public void Close()
	    {
	        Visible.Value = false;
        }
	    public void Open()
	    {
	        Visible.Value = true;
	    }

	    public void Place()
	    {
	        Close();
	        _prt.RunePreview = null;
	        _prt.runeId = SelectedDescription.ID;
            _mc.SelectTool<PlaceRuneTool>();

	    }

	    public void SetUnlockedRunes(Dictionary<string, int> ur)
	    {
            Debug.Log("unlocking");
	        foreach (var runeDescriptions in AllRunes)
	        {
	            foreach (var runeDescription in runeDescriptions)
	            {
	                runeDescription.SetUnlockedCount(ur);
                }
                runeDescriptions.ItemsModified(0, runeDescriptions.Count);
	        }
	    }

        public void Selected0()
	    {
            SetValue(() => SelectedDescription, Runes0.SelectedItem);
            
	    }
	    public void Selected1()
	    {
	        SetValue(() => SelectedDescription, Runes1.SelectedItem);
        }
	    public void Selected2()
	    {
	        SetValue(() => SelectedDescription, Runes2.SelectedItem);
        }
	    public void Selected3()
	    {
	        SetValue(() => SelectedDescription, Runes3.SelectedItem);
        }

		public void Wiki()
        {
            HeightScroll.Value = 5000;
            ContentViewSwitcher.SwitchTo(1);
		}

		public void Back()
        {
            HeightScroll.Value = 0;
            ContentViewSwitcher.Previous();
		}
    }

    public class RuneDescription
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Transliteration { get; set; }
        public string Phonetic { get; set; }
        public string Meaning { get; set; }
        public string Usage { get; set; }
		public string Wiki { get; set; }
        public int UnlockedCount = 0;
        public float Alpha = 0.2f;
        
        public void SetUnlockedCount(Dictionary<string, int> unlockedRunes)
        {
            UnlockedCount = unlockedRunes.ContainsKey(ID) ? unlockedRunes[ID] : 0;
            Alpha = UnlockedCount > 0 ? 1 : 0.2f;
        }

        public Sprite Image
        {
            get { return Resources.Load<Sprite>("Runes/Sprites/" + ID); }
            set { }
        }
    }
}