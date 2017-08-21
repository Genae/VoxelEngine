using System.Collections.Generic;

namespace Assets.Scripts.Logic.Classes
{
    public class Miner : Class
    {
        public override Dictionary<string, object> CreateGoalState()
        {
            var goalState = new Dictionary<string, object>
            {
                {"hasMined", true},
                {"hasBuilt", true},
                {"hasPlanted", true},
                {"hasHoed", true},
                {"hasHarvested", true}
            };
            return goalState;
        }
    }
}
