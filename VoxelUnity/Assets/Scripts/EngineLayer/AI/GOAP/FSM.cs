using System.Collections.Generic;
using UnityEngine;

namespace EngineLayer.AI.GOAP
{
    public class FSM
    {

        private readonly Stack<IState> _stateStack = new Stack<IState>();
        
        public void Update(GOAPAgent agent, GameObject obj)
        {
            if (_stateStack.Peek() != null)
            {
                _stateStack.Peek().Run(agent, this, obj);
            }
        }

        public void PushState(IState state)
        {
            _stateStack.Push(state);
        }

        public void PopState()
        {
            _stateStack.Pop();
        }
    }
}