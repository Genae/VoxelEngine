using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Logic.Actions;
using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public sealed class GOAPAgent : MonoBehaviour
    {
        private FSM _stateMachine;
        private FSM.FSMState _idleState;
        private FSM.FSMState _moveToState;
        private FSM.FSMState _performActionState;

        private HashSet<GOAPAction> _availableActions;
        private Queue<GOAPAction> _currentActions;
        private IGOAP _dataProvider;
        private GOAPPlanner _planner;


        // Use this for initialization
        void Start()
        {
            _stateMachine = new FSM();
            _currentActions = new Queue<GOAPAction>();
            _planner = new GOAPPlanner();
            FindDataProvider();
            CreateIdleState();
            CreateMoveToState();
            CreatePerformActionState();
            _stateMachine.PushState(_idleState);
            LoadActions();
        }

        // Update is called once per frame
        void Update()
        {
            _stateMachine.Update(gameObject);
        }

        public void AddAction(GOAPAction action)
        {
            _availableActions.Add(action);
        }

        public GOAPAction GetAction(Type action)
        {
            return _availableActions.FirstOrDefault(currAction => currAction.GetType() == action);
        }

        public void RemoveAction(GOAPAction action)
        {
            _availableActions.Remove(action);
        }

        private bool HasActionPlan()
        {
            return _currentActions.Count > 0;
        }

        private void CreateIdleState()
        {
            _idleState = (fsm, obj) => {

                var worldState = _dataProvider.GetWorldState();
                var goal = _dataProvider.CreateGoalState();

                var plan = _planner.Plan(gameObject, _availableActions, worldState, goal);
                if (plan != null)
                {
                    _currentActions = plan;
                    _dataProvider.PlanFound(goal, plan);

                    fsm.PopState();
                    fsm.PushState(_performActionState);
                }
                else
                {
                    _dataProvider.PlanFailed(goal);
                    fsm.PopState();
                    fsm.PushState(_idleState);
                }
            };
        }

        private void CreateMoveToState()
        {
            _moveToState = (fsm, go) => {

                var action = _currentActions.Peek();
                if (action.RequiresInRange() && action.Targets.Count == 0)
                {
                    fsm.PopState();
                    fsm.PopState();
                    fsm.PushState(_idleState);
                    return;
                }

                if (_dataProvider.MoveAgent(action))
                {
                    fsm.PopState();
                }

            };
        }

        private void CreatePerformActionState()
        {

            _performActionState = (fsm, obj) => {

                if (!HasActionPlan())
                {
                    fsm.PopState();
                    fsm.PushState(_idleState);
                    _dataProvider.ActionsFinished();
                    return;
                }

                var action = _currentActions.Peek();
                if (action.IsDone())
                {
                    _currentActions.Dequeue();
                }

                if (HasActionPlan())
                {
                    action = _currentActions.Peek();

                    if (!action.RequiresInRange() || action.IsInRange())
                    {
                        var success = action.Perform(Time.deltaTime, obj);
                        if (!success)
                        {
                            fsm.PopState();
                            fsm.PushState(_idleState);
                            CreateIdleState();
                            _dataProvider.PlanAborted(action);
                        }
                    }
                    else
                    {
                        fsm.PushState(_moveToState);
                    }
                }
                else
                {
                    fsm.PopState();
                    fsm.PushState(_idleState);
                    _dataProvider.ActionsFinished();
                }
            };
        }

        private void FindDataProvider()
        {
            foreach (var comp in gameObject.GetComponents(typeof(Component)))
            {
                var goap = comp as IGOAP;
                if (goap != null)
                {
                    _dataProvider = goap;
                    return;
                }
            }
        }

        private void LoadActions()
        {
            _availableActions = new HashSet<GOAPAction>
            {
                new MiningAction()
            };
        }
    }
}
