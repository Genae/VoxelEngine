using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheel : MonoBehaviour {

	
	void Update () {
        transform.RotateAroundLocal(Vector3.forward, -Time.deltaTime);
	}
}
