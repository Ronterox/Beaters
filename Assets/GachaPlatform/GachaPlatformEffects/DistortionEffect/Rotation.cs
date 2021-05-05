using System;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Rotation : MonoBehaviour {

    public Vector3 RotateAxis = new Vector3(1, 5, 10);
	public float speed;

	// Use this for initialization
	void Start () {
	
	}


	void Update () {
        transform.Rotate(RotateAxis * (speed * Time.deltaTime));
	}
}
