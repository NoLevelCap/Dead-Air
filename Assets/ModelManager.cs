using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour {

	float shift = .75f, range = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, shift + (range * .5f) + ((range * .5f) * Mathf.Sin(Time.fixedTime)));
	}
}
