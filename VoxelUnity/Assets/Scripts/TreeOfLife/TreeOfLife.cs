using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeOfLife : MonoBehaviour {

	Stem[] StemOfLife;
	Root[] RootsOfLife;

	public GameObject DebugPrefab;

	// Use this for initialization
	void Start () {
		//debug
		CreateStemOfLife(9, Random.Range (2, 4), 4f);

		foreach (Stem s in StemOfLife) {
			foreach (TreeNode tn in s.Nodes) {
				Instantiate (DebugPrefab, tn.position, Quaternion.identity);
			}

			for (int i = 0; i < s.Nodes.Length - 1; i++) {
				Debug.DrawLine (s.Nodes [i].position, s.Nodes [i + 1].position, Color.green, 10000f);
			}
		}
	}

	void CreateStemOfLife(int nodeCount, int stemCount, float minDistance){
		StemOfLife = new Stem[stemCount];
		Vector3 StemBase = new Vector3 (0, 0, 0);
		Vector3[] StemEnds = getStemEnds (stemCount, minDistance);
		StemOfLife[0] = new Stem(StemBase, StemEnds[0], nodeCount); // first = base Stem

		for (int i = 1; i < stemCount; i++) {
			var splitIndex = Random.Range (2, 6); //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			TreeNode branchStemBase = StemOfLife [0].Nodes [splitIndex];
			StemOfLife [i] = new Stem (branchStemBase, StemEnds[i], nodeCount-splitIndex);
		}
	}

	Vector3[] getStemEnds(int number, float minDistance){
		Vector3[] ends = new Vector3[number];
		if (number > 1) {
			float minDist = 0;
			while (minDist < minDistance) {
				minDist = float.MaxValue;
				for (int i = 0; i < number; i++) {
					ends [i] = (Random.onUnitSphere + Vector3.up * 2).normalized * 10;
				}
				for (int i = 0; i < number; i++) {
					for (int j = i + 1; j < number; j++) {
						var dist = Vector3.Distance (ends [i], ends [j]);
						if (minDist > dist) {
							minDist = dist;
						}
					}
				}
			}
		} else {
			ends [0] = (Random.onUnitSphere + Vector3.up * 2).normalized * 10;
		}
		return ends;
	}




	// Update is called once per frame
	void Update () {
		
	}
}
