using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.EngineLayer.AI.GOAP;
using Assets.Scripts.GameLogicLayer.Actions;

namespace Assets.Scripts.GameLogicLayer
{
    class JobAutoSolver : Class
    {

        public override Dictionary<string, object> CreateGoalState()
        {
            var goalState = new Dictionary<string, object>
            {
                {"hasMined", true},
                {"hasBuilt", true},
                {"hasPlanted", true},
                {"hasCreatedSoil", true}
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
                new PlantCropAction()
            };
            return actions;
        }

        public override bool MoveAgent(GOAPAction nextAction)
        {
            transform.position = nextAction.Targets.FirstOrDefault();
            return true;
        }
    }
}
