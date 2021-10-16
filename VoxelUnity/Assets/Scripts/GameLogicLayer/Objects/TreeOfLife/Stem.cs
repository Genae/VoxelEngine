using UnityEngine;

namespace GameLogicLayer.Objects.TreeOfLife
{
    public class Stem {
        public TreeNode[] Nodes;

        public Stem(Vector3 stemStart, Vector3 stemEnd, int length){
            Nodes = new TreeNode[length];
            Nodes[0] = new TreeNode (stemStart);
            Nodes[length-1] = new TreeNode(stemEnd);
            CreateStem (0, length-1);
        }

        public Stem(TreeNode tn, Vector3 stemEnd, int length){
            Nodes = new TreeNode[length];
            Nodes[0] = tn;
            Nodes[length-1] = new TreeNode(stemEnd);
            CreateStem (0, length-1);
        }

        void CreateStem(int first, int second){
            var mid = (first + second) / 2;
            if (mid == first)
                return;
            var midPos = midpointDisplacement (Nodes[first].position, Nodes[second].position);
            var midNode = new TreeNode(midPos);
            Nodes[mid] = midNode;
            CreateStem (first, mid);
            CreateStem (mid, second);
        }

        Vector3 midpointDisplacement(Vector3 first, Vector3 second){
            var dir = (second - first) / 2;
            var rnd = Random.insideUnitSphere * (dir.magnitude / 1.5f);
            rnd.y = 0;
            return first + dir + rnd;
        }

    }
}
