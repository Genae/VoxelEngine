using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public interface IFSMState
    {

        void Update(IFSMState fsm, GameObject obj);
    }
}
