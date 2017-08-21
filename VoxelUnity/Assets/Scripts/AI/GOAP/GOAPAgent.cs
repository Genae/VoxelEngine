using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Logic;
using Assets.Scripts.Logic.Actions;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public sealed class GOAPAgent : MonoBehaviour
    {
        internal FSM StateMachine;
        internal IState IdleState;
        internal IState MoveToState;
        internal IState PerformActionState;
        internal IState WaitForPlanState;

        internal HashSet<GOAPAction> AvailableActions;
        internal Queue<GOAPAction> CurrentActions;
        internal Class DataProvider;
        internal GOAPPlanner Planner;

        public bool IsIdle;
        public bool IsMoving;
        public GOAPlan Plan;

        // Use this for initialization
        void Start()
        {
            DataProvider = GetComponent<Class>();
            StateMachine = new FSM();
            CurrentActions = new Queue<GOAPAction>();
            Planner = new GOAPPlanner();
            IdleState = new IdleState();
            MoveToState = new MoveToState();
            PerformActionState = new PerformActionState();
            WaitForPlanState = new WaitingForPlanState();
            StateMachine.PushState(IdleState);
            LoadActions();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsIdle)
                return;
            
            StateMachine.Update(this, gameObject);
        }

        public void AddAction(GOAPAction action)
        {
            AvailableActions.Add(action);
        }

        public GOAPAction GetAction(Type action)
        {
            return AvailableActions.FirstOrDefault(currAction => currAction.GetType() == action);
        }

        public void RemoveAction(GOAPAction action)
        {
            AvailableActions.Remove(action);
        }

        internal bool HasActionPlan()
        {
            return CurrentActions.Count > 0;
        }
        
        private void LoadActions()
        {
            AvailableActions = new HashSet<GOAPAction>
            {
                new SolveJobAction("hasMined", JobType.Mining),
                new SolveJobAction("hasBuilt", JobType.Building),
                new SolveJobAction("hasPlanted", JobType.PlantCrop),
                new SolveJobAction("hasHoed", JobType.CreateSoil),
                new SolveJobAction("hasHarvested", JobType.HarvestCrop)
            };
        }
    }

    internal class PerformActionState : IState
    {
        public void Run(GOAPAgent agent, FSM fsm, GameObject actor)
        {

            if (!agent.HasActionPlan())
            {
                fsm.PopState();
                fsm.PushState(agent.IdleState);
                agent.DataProvider.ActionsFinished();
                return;
            }

            var action = agent.CurrentActions.Peek();
            if (action.IsDone())
            {
                agent.CurrentActions.Dequeue();
            }

            if (agent.HasActionPlan())
            {
                action = agent.CurrentActions.Peek();

                if (!action.RequiresInRange() || action.IsInRange())
                {
                    var success = action.Perform(Time.deltaTime, agent.gameObject);
                    if (!success)
                    {
                        fsm.PopState();
                        fsm.PushState(agent.IdleState);
                        agent.DataProvider.PlanAborted(action);
                    }
                }
                else
                {
                    fsm.PushState(agent.MoveToState);
                }
            }
            else
            {
                fsm.PopState();
                fsm.PushState(agent.IdleState);
                agent.DataProvider.ActionsFinished();
            }
        }
    }

    internal class MoveToState : IState
    {
        public void Run(GOAPAgent agent, FSM fsm, GameObject actor)
        {

            var action = agent.CurrentActions.Peek();
            if (action.RequiresInRange() && action.Targets.Count == 0)
            {
                fsm.PopState();
                fsm.PopState();
                fsm.PushState(agent.IdleState);
                return;
            }

            if (agent.DataProvider.MoveAgent(action))
            {
                fsm.PopState();
            }

        }
    }

    internal class IdleState : IState
    {
        public void Run(GOAPAgent agent, FSM fsm, GameObject actor)
        {
            var worldState = agent.DataProvider.GetWorldState();
            var goal = agent.DataProvider.CreateGoalState();

            agent.Plan = agent.Planner.Plan(agent.gameObject, agent.AvailableActions, worldState, goal);
            fsm.PopState();
            fsm.PushState(agent.WaitForPlanState);
        }
    }

    internal class WaitingForPlanState : IState
    {
        public void Run(GOAPAgent agent, FSM fsm, GameObject actor)
        {
            if(!agent.Plan.Finished)
                return;

            var plan = agent.Plan;
            if (plan != null && plan.Actions != null && plan.Success)
            {
                agent.CurrentActions = plan.Actions;
                agent.DataProvider.PlanFound(plan.Goal, plan.Actions);

                fsm.PopState();
                fsm.PushState(agent.PerformActionState);
            }
            else
            {
                agent.DataProvider.PlanFailed(plan.Goal);
                fsm.PopState();
                fsm.PushState(agent.IdleState);
                JobController.Instance.AddIdleSolver(agent.DataProvider);
                agent.IsIdle = true;
            }
        }
    }
}
