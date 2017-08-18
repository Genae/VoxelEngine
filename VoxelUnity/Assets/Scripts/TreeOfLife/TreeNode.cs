using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode {
	public Vector3 position;
	public int thickness;
	TreeNode prev;
	List<TreeNode> next;

	public TreeNode(Vector3 pos){
		position = pos;
	}
}
