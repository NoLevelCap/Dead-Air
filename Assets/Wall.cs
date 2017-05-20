using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public Mesh tallW, shortW;
	private MeshFilter cWall;
	public LeaveObject door;
	private bool isWallDown = false;

	// Use this for initialization
	void Start () {
		cWall = this.gameObject.GetComponent<MeshFilter> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool canMove(){
		//Debug.Log ("Move attempt " + (door != null));
		if(door != null){
			return true;
		}
		return false;
	}

	public LeaveObject getLeaveObj(){
		return door;
	}

	public void toggleWall(){
		toggleWall (!isWallDown);
	}

	public void toggleWall(bool setting){
		isWallDown = setting;
		if (isWallDown) {
			getCWall().mesh = shortW;
		} else {
			getCWall().mesh = tallW;
		}
	}

	public bool isWallD(){
		return isWallDown;
	}

	MeshFilter getCWall(){
		if(cWall == null){
			cWall = this.gameObject.GetComponent<MeshFilter> ();
		}
		return cWall;
	}
}
