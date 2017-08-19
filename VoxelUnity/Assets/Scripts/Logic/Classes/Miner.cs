using System.Collections.Generic;

namespace Assets.Scripts.Logic.Classes
{
    public class Miner : Class
    {
        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            var goalState = new HashSet<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("hasMined", true),
                new KeyValuePair<string, object>("hasBuilt", true),
                new KeyValuePair<string, object>("hasPlanted", true),
                new KeyValuePair<string, object>("hasHoed", true),
                new KeyValuePair<string, object>("hasHarvested", true)
            };
            return goalState;
        }
    }
}
