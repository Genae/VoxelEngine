using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.UI;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class CampaignManager : MonoBehaviour
    {
        public static CampaignManager Instance;
        public int CurrentLevel = 0;
        private int _currentWave = 0;
        private Dictionary<int, Level> _levels;
        public readonly Dictionary<string, int> UnlockedRunes = new Dictionary<string, int>();
        public readonly List<string> NewlyUnlocked = new List<string>();
        public Dictionary<string, int> UnlockedThisTime = new Dictionary<string, int>();

        // Use this for initialization
        void Awake()
        {
            Instance = this;
            _levels = ConfigImporter.GetAllConfigs<Level>("Configs/Campaign").ToDictionary(l => l.LevelNumber);
        }
        
        void Start()
        {
            SetLevel(CurrentLevel);
        }

        private void UnlockRunes()
        {
            UnlockedRunes.Clear();
            UnlockedRunes["raido"] = 4;
            if (CurrentLevel == 0)
            {
                NewlyUnlocked.Add("raido");
                UnlockedThisTime["raido"] = 4;
            }
            else
            {
                UnlockedThisTime = _levels[CurrentLevel-1].UnlockedRunes.ToDictionary(ur => ur.Name, ur => ur.Amount);
            }
            for (var i = 0; i < CurrentLevel; i++)
            {
                if (_levels.ContainsKey(i))
                {
                    foreach (var unlockedRune in _levels[i].UnlockedRunes)
                    {
                        if (!UnlockedRunes.ContainsKey(unlockedRune.Name))
                        {
                            UnlockedRunes[unlockedRune.Name] = 0;
                            if(i == CurrentLevel - 1)
                                NewlyUnlocked.Add(unlockedRune.Name);
                        }
                        UnlockedRunes[unlockedRune.Name] += unlockedRune.Amount;
                    }
                }
            }
            RuneOverview.Instance.SetUnlockedRunes(UnlockedRunes);
        }


        public Wave GetNextWave()
        {
            Debug.Log("getting wave " +  _currentWave + " in level " + CurrentLevel);
            if (_levels[CurrentLevel].Waves.Length <= _currentWave)
            {
                if(WaveManager.AliveMinions.Count == 0)
                    SetLevel(CurrentLevel + 1);
                return null;
            }
            return _levels[CurrentLevel].Waves[_currentWave++];
        }

        public void SetLevel(int level)
        {
            Debug.Log(level);
            CurrentLevel = level;
            _currentWave = 0;
            if(TDMap.Instance != null)
                TDMap.Instance.Clear();
            CampaignText.Instance.Text.Value = _levels[level].Text;
            CampaignText.Instance.Visible.Value = true;
            UnlockRunes();
        }

        public MapInfo GetMapInfo()
        {
            return _levels[CurrentLevel].MapInfo;
        }
    }

    public class MapInfo
    {
        public float[][] Path;
        public float[] Village;

        public List<Vector3> GetPath(MapSize size)
        {
            var list = new List<Vector3>();
            foreach (var pos in Path)
            {
                list.Add(new Vector3(pos[0]*size.Width + size.MinX, 0, pos[1]*size.Heigth + size.MinZ));
            }
            if(Village.Length != 0)
                list.Add(new Vector3(Village[0] * size.Width + size.MinX, 0, Village[1] * size.Heigth + size.MinZ));
            return list;
        }

        public Vector3 GetVillagePos(MapSize size)
        {
            if(Village.Length == 0)
                return Vector3.zero;
            return new Vector3(Village[0] * size.Width + size.MinX, 0.01f, Village[1] * size.Heigth + size.MinZ);
        }
    }

    public class Level
    {
        public Wave[] Waves;
        public int LevelNumber;
        public string Text;
        public UnlockedRune[] UnlockedRunes;
        public MapInfo MapInfo;
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
        public string Model;
        public float Scale;
        public ElementType[] ElementList;
    }
}