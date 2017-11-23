using UnityEngine;
using Assets.Scripts.EngineLayer;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class CampaignManager : MonoBehaviour
    {

        private int _currentLevel = 0;
        private int _currentWave = 0;
        private Level[] _levels;

        // Use this for initialization
        void Awake()
        {
            _levels = ConfigImporter.GetAllConfigs<Level>("Configs/Campaign").ToArray();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Wave GetNextWave()
        {
            return _levels[_currentLevel].Waves[_currentWave++];
        }
    }

    public class Level
    {
        public Wave[] Waves;
        //Vector3 list path
        //string flavortext
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