﻿using System.Linq;
using Assets.Scripts.ControlInputs;
using Assets.Scripts.EngineLayer;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class RuneOverview : UIView
	{
        public ObservableList<RuneDescription>[] AllRunes = new ObservableList<RuneDescription>[4];
	    public ObservableList<RuneDescription> Runes0, Runes1, Runes2, Runes3;
	    public RuneDescription SelectedDescription;
        

        void Awake()
        {
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
        }

	    public void Selected0()
	    {
            SetValue(() => SelectedDescription, Runes0.SelectedItem);
            Debug.Log(SelectedDescription.Name);
            
	    }
	    public void Selected1()
	    {
	        SetValue(() => SelectedDescription, Runes1.SelectedItem);
	        Debug.Log(SelectedDescription.Name);
        }
	    public void Selected2()
	    {
	        SetValue(() => SelectedDescription, Runes2.SelectedItem);
	        Debug.Log(SelectedDescription.Name);
        }
	    public void Selected3()
	    {
	        SetValue(() => SelectedDescription, Runes3.SelectedItem);
	        Debug.Log(SelectedDescription.Name);
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
        public string Image
        {
            get { return "Assets/Resources/Runes/Sprites/" + ID + ".png"; }
            set { }
        }
    }
}