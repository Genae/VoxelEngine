using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AI.GOAP;
using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Algorithms.Pathfinding.Pathfinder;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public abstract class Class : MonoBehaviour, IGOAP
    {
        private JobController _jobController;
        public Path PathToTarget;
        public float MoveSpeed;
        private int _pathIndex;
        private Node _currentNode;

        void Start()
        {
            if (_jobController == null)
            {
                _jobController = GameObject.Find("World").GetComponent<JobController>();
            }
        }
        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            var worldData = new HashSet<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("hasMined", false),
                new KeyValuePair<string, object>("hasBuilt", false)
            };
            return worldData;
        }

        public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();

        public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
        {
            ResetState();
        }

        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
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

        public bool MoveAgent(GOAPAction nextAction)
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

            if (PathToTarget.Targets.Any(t => (t.Position - transform.position).magnitude < 0.6f))
            {
                nextAction.SetInRange(true);
                return true;
            }

            if (_currentNode == null)
            {
                _currentNode = PathToTarget.GetNode(0);
                if (_currentNode == null)
                {
                    PathToTarget = null;
                    nextAction.SetInRange(true);
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
