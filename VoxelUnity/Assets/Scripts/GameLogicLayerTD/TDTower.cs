using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDTower{

	private Tower _tower;

	public TDTower(Vector3 pos){
		_tower = new GameObject("Tower").AddComponent<Tower>();
		_tower.transform.position = pos;
		_tower.transform.parent = GameObject.Find("Map").transform;
		var cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		//TODO Placeholder stuff
		cube.transform.parent = _tower.transform;
		cube.transform.localPosition = Vector3.zero;
		cube.transform.localScale = new Vector3 (4, 15, 4);
		cube.transform.name = "TowerMesh";
	}
}
