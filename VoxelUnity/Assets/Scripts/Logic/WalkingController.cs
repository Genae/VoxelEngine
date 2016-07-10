using Assets.Scripts.Algorithms.Pathfinding;
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

        void Update ()
        {
            if (PathToTarget == null || !PathToTarget.Finished || PathToTarget.Nodes == null)
            {
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
                    transform.FindChild("Scale").LookAt(_currentNode.Position);
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
    }
}
