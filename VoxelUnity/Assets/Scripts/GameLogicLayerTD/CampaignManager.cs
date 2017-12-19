﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.UI;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class CampaignManager : MonoBehaviour
    {

        public int CurrentLevel = 0;
        private int _currentWave = 0;
        private Dictionary<int, Level> _levels;
        public readonly Dictionary<string, int> UnlockedRunes = new Dictionary<string, int>(); 

        // Use this for initialization
        void Awake()
        {
            _levels = ConfigImporter.GetAllConfigs<Level>("Configs/Campaign").ToDictionary(l => l.LevelNumber);
        }
        
        void Start()
        {
            UnlockRunes();
        }

        private void UnlockRunes()
        {
            UnlockedRunes.Clear();
            UnlockedRunes["mannaz"] = 1;
            UnlockedRunes["algiz"] = 2;
            UnlockedRunes["raido"] = 4;
            for (var i = 0; i < CurrentLevel; i++)
            {
                if (_levels.ContainsKey(i))
                {
                    foreach (var unlockedRune in _levels[i].UnlockedRunes)
                    {
                        if (!UnlockedRunes.ContainsKey(unlockedRune.Name))
                        {
                            UnlockedRunes[unlockedRune.Name] = 0;
                        }
                        UnlockedRunes[unlockedRune.Name] += unlockedRune.Amount;
                    }
                }
            }
            RuneOverview.Instance.SetUnlockedRunes(UnlockedRunes);
        }


        public Wave GetNextWave()
        {
            return _levels[CurrentLevel].Waves[_currentWave++];
        }

        public void SetLevel(int level)
        {
            CurrentLevel = level;
        }
    }

    public class Level
    {
        public Wave[] Waves;
        public int LevelNumber;
        public UnlockedRune[] UnlockedRunes;
        //Vector3 list path
        //string flavortext
    }

    public class UnlockedRune
    {
        public string Name;
        public int Amount;
    }

    public class Wave
    {
        public MinionConfiguration[] MobLookup;
        public int[][] SpawnPatterns;
        private int patternIndex;
        private int patternCount;
        private int internalPatternIndex;

        public MinionConfiguration GetMobStats()
        {
            if (internalPatternIndex == SpawnPatterns[patternIndex].Length - 1)
            {
                patternCount++;
                internalPatternIndex = 0;
                if (patternCount == SpawnPatterns[patternIndex][0])
                {
                    patternIndex++;
                    patternCount = 0;
                    internalPatternIndex = 0;
                }
            }
            if (patternIndex >= SpawnPatterns.Length)
                return null;
            internalPatternIndex++;
            return MobLookup[SpawnPatterns[patternIndex][internalPatternIndex]];
        }
    }

    public class MinionConfiguration
    {
        public float Speed;
        public float Health;
        public float Scale;
        public ElementType[] ElementList;
    }
}