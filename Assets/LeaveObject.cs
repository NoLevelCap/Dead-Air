using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveObject : MonoBehaviour {

	public LeaveType type = LeaveType.PassThrough;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum LeaveType
{
	Empty, PassThrough, Block
}
