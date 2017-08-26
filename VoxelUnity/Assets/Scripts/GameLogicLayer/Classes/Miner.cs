using System.Collections.Generic;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.EngineLayer.AI.GOAP;
using Assets.Scripts.GameLogicLayer.Actions;

namespace Assets.Scripts.GameLogicLayer.Classes
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
                {"hasCreatedSoil", true},
                {"hasHarvested", true}
            };
            return goalState;
        }

        public override List<GOAPAction> GetPossibleActions()
        {
            var actions = new List<GOAPAction>
            {
                new BuildingAction(),
                new MiningAction(),
                new CreateSoilAction(),
                new HarvestCropAction(),
                new PlantCropAction()
            };
            return actions;
        }
    }
}
