using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public interface IState
    {
        void Run(GOAPAgent agent, FSM fsm, GameObject actor);
    }
}
