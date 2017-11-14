using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {

	private List<Transform> _towerList;
	private Transform _markerFolder;
	private Transform _tower;
	private LineRenderer _lr;

	void Start () {
		_markerFolder = transform.parent;
		_towerList = new List<Transform> ();
		//linerenderer init
		_lr = gameObject.AddComponent<LineRenderer> ();
		_lr.positionCount = 2;
		var mat = new Material (Shader.Find ("Standard"));
		mat.color = Color.black;
		_lr.material = mat;
	}
	
	void Update () {
		
		GetTowers();
		GetClosestTower();
		ConfigLineRenderer();
	}

	private void GetTowers(){
		_towerList.Clear ();
		foreach (var t in _markerFolder.GetComponentsInChildren<Transform>()) {
			if (t.name.Contains ("Tower Base")) {
				_towerList.Add (t);
			}
		}
	}

	private void GetClosestTower(){
		var dist = Mathf.Infinity;
		Transform tower;
		foreach (var t in _towerList) {
			if (Vector3.Distance (transform.position, t.position) < dist) {
				_tower = t;
				dist = Vector3.Distance (transform.position, t.position); //awesome
			}
		}
	}

	private void ConfigLineRenderer(){
		_lr.SetPosition(0, transform.position);
		_lr.SetPosition(1, _tower.position);
		_lr.startWidth = 1;
		_lr.endWidth = 1;
		//_lr.startColor = Color.black;
		//_lr.endColor = Color.black;
	}
}
