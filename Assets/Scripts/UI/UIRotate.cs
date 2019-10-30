using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour {
    public Vector3 rotation;


	void Start () {
		
	}
	

	void Update () {
        transform.Rotate(rotation * Time.deltaTime);
	}
}
