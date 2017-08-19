using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public class FSM
    {

        private readonly Stack<FSMState> _stateStack = new Stack<FSMState>();

        public delegate void FSMState(FSM fsm, GameObject obj);

        public void Update(GameObject obj)
        {
            if (_stateStack.Peek() != null)
            {
                _stateStack.Peek().Invoke(this, obj);
            }
        }

        public void PushState(FSMState state)
        {
            _stateStack.Push(state);
        }

        public void PopState()
        {
            _stateStack.Pop();
        }
    }
}