using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour {

	public PlayerController pc;

	private int TurnCounterINT = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(pc.getAP() <= 0){
			newTurn();
		}
	}

	void newTurn(){
		TurnCounterINT++;
		//Debug.Log ("Starting turn " + TurnCounterINT + ".");
		pc.newTurn ();
	}
}
