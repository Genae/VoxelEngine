using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Algorithms.Pathfinding.Pathfinder;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class WalkingController : MonoBehaviour
    {
        public Path PathToTarget;
        public float MoveSpeed;
        private int _pathIndex;
        private Node _currentNode;

        private Map _map;

        public bool IsIdle
        {
            get { return PathToTarget == null; }
        }

        void Start()
        {
            if (_map == null)
            {
                _map = Map.Instance;
            }
        }

        void Update ()
        {
            if (PathToTarget == null || !PathToTarget.Finished || PathToTarget.State != PathState.Ready)
            {
                if (PathToTarget != null && PathToTarget.State == PathState.Invalid)
                {
                    PathToTarget.Dispose();
                    PathToTarget = null;
                }
                return;
            }
            if (_currentNode == null)
            {
                _currentNode = PathToTarget.GetNode(0);
                _pathIndex = 0;
            }
            PathToTarget.Visualize(Color.red, _pathIndex);
            var moveDist = MoveSpeed*Time.deltaTime;
            while (moveDist > 0)
            {
                if ((transform.position - _currentNode.Position).magnitude > moveDist)
                {
                    transform.Translate((_currentNode.Position - transform.position).normalized * moveDist, Space.Self);
                    transform.Find("Scale").LookAt(_currentNode.Position);
                    return;
                }
                else
                {
                    moveDist -= (transform.position - _currentNode.Position).magnitude;
                    transform.position = _currentNode.Position;
                    _currentNode = PathToTarget.GetNode(++_pathIndex);
                    if (_currentNode == null)
                    {
                        PathToTarget = null;
                        return;
                    }
                }
            }
        }

        public void MoveTo(Vector3 target)
        {
            Debug.Log("Calculating path");
            PathToTarget = Path.Calculate(_map.AStarNetwork, transform.position, target, true);
        }
    }
}
