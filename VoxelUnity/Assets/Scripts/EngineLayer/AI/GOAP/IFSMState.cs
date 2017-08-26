using UnityEngine;

namespace Assets.Scripts.EngineLayer.AI.GOAP
{
    public interface IState
    {
        void Run(GOAPAgent agent, FSM fsm, GameObject actor);
    }
}
