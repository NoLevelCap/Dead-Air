using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour {

	MapGen mp;
	Renderer r;
	public Texture2D obj;


	// Use this for initialization
	void Start () {
		mp = GetComponent<MapGen> ();
		r = GetComponent<Renderer> ();
		r.material.mainTexture = obj;
		obj = mp.NoiseTex;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E)) {
			obj = mp.NoiseTex;
		}
	}
}
