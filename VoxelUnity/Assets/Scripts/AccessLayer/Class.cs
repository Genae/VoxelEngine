using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer.Jobs;
using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Algorithms.Pathfinding.Pathfinder;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.EngineLayer.AI.GOAP;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;

namespace Assets.Scripts.AccessLayer
{
    public abstract class Class : MonoBehaviour, IGOAP
    {
        private GOAPAgent _agent;
        public Path PathToTarget;
        public float MoveSpeed;
        private int _pathIndex;
        private Node _currentNode;

        void Start()
        {
            _agent = GetComponent<GOAPAgent>();
        }
        
        public Dictionary<string, object> GetWorldState()
        {
            var worldData = new Dictionary<string, object>
            {
                {"hasMined", false},
                {"hasBuilt", false},
                {"hasPlanted", false},
                {"hasCreatedSoil", false},
                {"hasHarvested", false}
            };
            return worldData;
        }

        public void JobAvailable()
        {
            _agent.IsIdle = false;
        }

        public abstract List<GOAPAction> GetPossibleActions();

        public string[] GetPossibleJobs()
        {
            return GetPossibleActions().Where(a => a is SolveJobAction).Select(a => ((SolveJobAction) a).Type).ToArray();
        }

        public abstract Dictionary<string, object> CreateGoalState();

        public void PlanFailed(Dictionary<string, object> failedGoal)
        {
            ResetState();
        }

        public void PlanFound(Dictionary<string, object> goal, Queue<GOAPAction> actions)
        {
            foreach (var goapAction in actions)
            {
                goapAction.HasBeenChoosen();
            }
        }

        public void ActionsFinished()
        {
            ResetState();
        }

        public void PlanAborted(GOAPAction aborter)
        {
            ResetState();
        }

        private void ResetState()
        {
            if (PathToTarget != null)
            {
                PathToTarget.Dispose();
                PathToTarget = null;
                _currentNode = null;
                _pathIndex = 0;
            }
        }

        public virtual bool MoveAgent(GOAPAction nextAction)
        {
            if (PathToTarget == null || !PathToTarget.Finished || PathToTarget.State != PathState.Ready)
            {
                if (PathToTarget == null)
                {
                    PathToTarget = Path.Calculate(Map.Instance.AStarNetwork, transform.position, nextAction.Targets.Select(t => new Vector3I(t)).ToList(), true);
                }
                if (PathToTarget != null && PathToTarget.State == PathState.Invalid)
                {
                    PathToTarget.Dispose();
                    PathToTarget = null;
                }
                return false;
            }

            if (nextAction.IsInRange(_agent))
            {
                return true;
            }

            if (_currentNode == null)
            {
                _currentNode = PathToTarget.GetNode(0);
                if (_currentNode == null)
                {
                    PathToTarget = null;
                    return true;
                }
                _pathIndex = 0;
            }
            PathToTarget.Visualize(Color.red, _pathIndex);
            var moveDist = MoveSpeed * Time.deltaTime;
            while (moveDist > 0)
            {
                if ((transform.position - _currentNode.Position).magnitude > moveDist)
                {
                    transform.Translate((_currentNode.Position - transform.position).normalized * moveDist, Space.Self);
                    transform.Find("Scale").LookAt(_currentNode.Position);
                    return false;
                }
                else
                {
                    moveDist -= (transform.position - _currentNode.Position).magnitude;
                    transform.position = _currentNode.Position;
                    _currentNode = PathToTarget.GetNode(++_pathIndex);
                    if (_currentNode == null)
                    {
                        PathToTarget = null;
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
