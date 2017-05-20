using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

	public float Offset_X, Offset_Y;
	public Wall n, s, w, e;
	[SerializeField]
	private bool _inRoom;

	[SerializeField]
	private RoomScript Action; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void activate(){
		Debug.Log ("Action Arrived");
		if(Action != null){
			Debug.Log ("Action Object Called");
			Action.OnAction ();
		}
	}

	public int doorNumber(){
		bool[] b = canWalk ();
		int a = 0;
		for (int i = 0; i < b.Length; i++) {
			if(b[i]){
				a++;
			}
		}
		return a;
	}

	public float doorChance(){
		int a = doorNumber();
		if (a == 4) {
			return 1f;
		} else if (a == 3) {
			return .5f;
		} else if (a == 2) {
			return .2f;
		} else if (a == 1) {
			return .1f;
		}
		return 0f;
	}

	public GameObject getDecor(){
		return this.gameObject;
	}

	/*
	 *  Gets walk direction. NESW
	 * 
	 */
	public bool[] canWalk(){
		bool[] b = new bool[4];
		if (n != null) { b [0] = n.canMove(); } else { b [0] = true; }
		if (e != null) { b [1] = e.canMove(); } else { b [1] = true; }
		if (s != null) { b [2] = s.canMove(); } else { b [2] = true; }
		if (w != null) { b [3] = w.canMove(); } else { b [3] = true; }
		return b;
	}

	public LeaveType[] getWalk(){
		LeaveType[] b = new LeaveType[4];

		if (n != null) { if(n.canMove()){ b[0] = n.getLeaveObj ().type; } else { b [0] = LeaveType.Block; } } else { b [0] = LeaveType.Empty; }
		if (e != null) { if(e.canMove()){ b[1] = e.getLeaveObj ().type; } else { b [1] = LeaveType.Block; } } else { b [1] = LeaveType.Empty; }
		if (s != null) { if(s.canMove()){ b[2] = s.getLeaveObj ().type; } else { b [2] = LeaveType.Block; } } else { b [2] = LeaveType.Empty; }
		if (w != null) { if(w.canMove()){ b[3] = w.getLeaveObj ().type; } else { b [3] = LeaveType.Block; } } else { b [3] = LeaveType.Empty; }
		return b;
	}

	public bool inRoom {
		get {
			return this._inRoom;
		}
		set {
			_inRoom = value;
			if (Action != null) {
				if (_inRoom) {
					Action.OnEnter ();
				} else {
					Action.OnLeave ();
				}
			}
		}
	}
}
