using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDTower{

	public Tower Tower;

	public TDTower(Vector3 pos){
		Tower = new GameObject("Tower").AddComponent<Tower>();
		Tower.transform.position = pos;
		Tower.transform.parent = GameObject.Find("Map").transform;
		var cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//TODO Placeholder stuff
		cube.transform.parent = Tower.transform;
		cube.transform.localPosition = Vector3.zero;
		cube.transform.localScale = new Vector3 (4, 15, 4);
		cube.transform.name = "TowerMesh";
	}
}
