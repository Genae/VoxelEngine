using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Objects.TreeOfLife
{
    public class TreeNode {
        public Vector3 position;
        public float thickness;
        TreeNode prev;
        List<TreeNode> next;

        public TreeNode(Vector3 pos){
            position = pos;
        }
    }
}
