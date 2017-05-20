using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public RoomRenderer rr;
	private int x, y;
	private Room cRoom;

	private int ap = 0;

	Transform position;

	// Use this for initialization
	void Start () {
		Vector2 pos = rr.CreatePlayerSpawn ();
		rr.setRoomPlayer (this, x, y);
		x = Mathf.FloorToInt (pos.x);
		y = Mathf.FloorToInt (pos.y);
		position = this.transform;
		position.position = new Vector3 (x * 14f, position.position.y, y * 14f);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.W)){
			MoveV (true);
		} else if(Input.GetKeyDown(KeyCode.S)){
			MoveV (false);
		} else if(Input.GetKeyDown(KeyCode.D)){
			MoveH (true);
		} else if(Input.GetKeyDown(KeyCode.A)){
			MoveH (false);
		}

		if(Input.GetKeyDown(KeyCode.E)){
			cRoom.manager.activate ();
		}
	}

	void MoveV(bool up){
		if(up){
			if(rr.load(this, 0, 1)){
				y++;
				Move ();
			}
		} else {
			if(rr.load(this, 0, -1)){
				y--;
				Move ();
			}
		}
	}

	void Move(){
		position.position = new Vector3 (x * 14f + cRoom.manager.Offset_X, position.position.y, y * 14f + cRoom.manager.Offset_Y);
	}

	void MoveH(bool right){
		if(right){
			if(rr.load(this, +1, 0)){
				x++;
				Move ();
			}
		} else {
			if(rr.load(this, -1, 0)){
				x--;
				Move ();
			}
		}
	}

	public void newTurn(){
		ap = 30;
	}

	public int getX(){
		return x;
	}

	public int getY(){
		return y;
	}

	public int getAP(){
		return ap;
	}

	public void setRoom(Room r){
		cRoom = r;
	}

	public void removeAllAP(){
		ap = 0;
	}

	public void removeAP(int p){
		ap -= p;
	}
}
