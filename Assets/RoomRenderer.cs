using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomRenderer : MonoBehaviour {


	private MapGen AreaTex;

	public GameObject E_Out, E_In, E_Stu;

	public GameObject empty, start, basic;

	public RoomSet Office;

	private Room[,] grid;
	private int mx = 32, my = 32;

	private Room lRoom;


	// Use this for initialization
	void Start () {
		AreaTex = GetComponent<MapGen> ();
		grid = new Room[mx,my];
		preload ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool load(PlayerController pc, int dx, int dy){
		int x = pc.getX () + dx;
		int y = pc.getY () + dy;
		if (x >= 0 && x < mx && y >= 0 && y < my && pc.getAP () > 0) {
			if (grid [pc.getX (), pc.getY ()].canMoveDirection (dx, dy)) {
				if (!grid [x, y].chosen) {
					replace (pickRoom(Office, x, y, getDirection(dx, dy)), x, y);
					grid [x, y].chosen = true;
					pc.removeAllAP ();
				} else {
					pc.removeAP (1);
				}

				setRoomPlayer (pc, x, y);
				move (pc.getX(), pc.getY(), x, y);
				lRoom.manager.inRoom = false;
				grid [x, y].manager.inRoom = true;
				lRoom = grid [x, y];


				return true;
			} else {
				Debug.Log ("No Movement");
			}
		} else {
			Debug.Log ("Out of Bounds");
		}
		return false;
	}

	void move(int ox, int oy, int nx, int ny){
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if(ox+x >= 0 && ox+x < mx && oy+y >= 0 && oy+y < my){
					grid [ox + x, oy + y].heightenWalls ();
				}
			}
		}

		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if(nx+x >= 0 && nx+x < mx && ny+y >= 0 && ny+y < my){
					if (!(x == -1 || y == 1)) {
						grid [nx + x, ny + y].shortenWalls (true);
					}
				}
			}
		}

		grid [nx, ny].heightenWalls ();
		grid [nx, ny].shortenWalls (false);
	}

	void setRoomPlayer(PlayerController pc, Room room){
		pc.setRoom (room);
	}

	public void setRoomPlayer(PlayerController pc, int x, int y){
		pc.setRoom (grid[x,y]);
	}

	GameObject pickRoom(RoomSet CurrentSet, int nx, int ny, Direction dir){
		List<int> valid = new List<int>();
		List<PlaceRoomInfo> surroundingRooms = new List<PlaceRoomInfo>();
		Room oldRoom;
		if (dir == Direction.North) {
			oldRoom = grid [nx, ny - 1];
		} else if (dir == Direction.South) {
			oldRoom = grid [nx, ny + 1];
		} else if (dir == Direction.East) {
			oldRoom = grid [nx - 1, ny];
		} else if (dir == Direction.West) {
			oldRoom = grid [nx + 1, ny];
		} else {
			oldRoom = grid [nx, ny + 1];
		}

		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if (nx + x >= 0 && nx + x < mx && ny + y >= 0 && ny + y < my && !(y == x) && (y == 0 || x == 0)) {
					if(grid[nx + x, ny + y].chosen){
						surroundingRooms.Add (new PlaceRoomInfo(grid[nx + x, ny + y], getDirection(x, y)));
					}
				}
			}
		}


		//Debug.Log ("Started room pick " + (CurrentSet.Special.ToArray().Length + CurrentSet.Basic.ToArray().Length));

		List<GameObject> chosen;
		bool Special = false, V_Special = true;

		if (CurrentSet.Special.ToArray ().Length > 0) {
			chosen = CurrentSet.Special;
			Special = true;
		} else {
			chosen = CurrentSet.Basic;
		}


		do {
			if(!V_Special){
				chosen = CurrentSet.Basic;
				Special = false;
			}

			for (int i = 0; i < chosen.ToArray ().Length; i++) {
				bool V_FLAG = true;
				RoomManager m = chosen [i].GetComponent<RoomManager> ();
				bool[] c = m.canWalk ();
				LeaveType[] b = m.getWalk ();

				int dnum = getOppDirNum (getDirNum ((dir)));
				if (b [dnum] == LeaveType.Block) {
					V_FLAG = false;
				} 

				if (nx <= 0) {
					if (c [getDirNum (Direction.West)]) {
						V_FLAG = false;
					}
				} else if (nx >= mx - 1) {
					if (c [getDirNum (Direction.East)]) {
						V_FLAG = false;
					}
				}

				if (ny <= 0) {
					if (c [getDirNum (Direction.South)]) {
						V_FLAG = false;
					}
				} else if (ny >= my - 1) {
					if (c [getDirNum (Direction.North)]) {
						V_FLAG = false;
					}
				}
				if(oldRoom.chosen){
					if(oldRoom.manager.doorNumber() == 1 && m.doorNumber() == 1){
						V_FLAG = false;
					}
				}

				foreach (PlaceRoomInfo cr in surroundingRooms) {
					//Debug.Log ("Room exit type " + cr.getRoomsExitType() + "/" + cr.dir + "/" + b[getDirNum(cr.dir)]);
					if ((cr.getRoomsExitType () == LeaveType.Block && b [getDirNum (cr.dir)] != LeaveType.Block) || (cr.getRoomsExitType () != LeaveType.Block && b [getDirNum (cr.dir)] == LeaveType.Block)) {
						V_FLAG = false;
					}
				}


				if (V_FLAG) {
					valid.Add (i);
				} 
			}
			if(Special && (valid.ToArray ().Length == 0)){
				V_Special = false;
			}
		} while (!V_Special && Special);

		int size = valid.ToArray ().Length;
		if(size > 0){
			int ni = Random.Range (0, size);
			GameObject nr = chosen [valid [ni]];
			if(Special){
				CurrentSet.Special.Remove (nr);
			}
			return nr;
		} 
		return basic;
		
	}

	public static int getOppDirNum(int r){
		r += 2;
		if(r > 3){
			r -= 4;
		}
		return r;
	}

	public static Direction getDirection(int dx, int dy){
		if(dx > 0){
			return Direction.East;
		} else if(dx < 0){
			return Direction.West;
		}  else if(dy > 0){
			return Direction.North;
		}  else if(dy < 0){
			return Direction.South;
		}
		Debug.Log ("Null Direction.");
		return Direction.NULL;
	}

	public static int getDirNum(Direction dir){
		switch (dir) {
		case Direction.North:
			return 0;
		case Direction.East:
			return 1;
		case Direction.South:
			return 2;
		case Direction.West:
			return 3;
		default:
			Debug.Log ("Attempt to get Direction Number on a Null Direction.");
			return -1;
		}
	}

	void preload(){
		int w = AreaTex.pixWidth;
		Color[] map = AreaTex.MapColors;
		for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				grid [x,y] = new Room(roomload (empty, x, y), x, y);


				float sqaure = map[(y * w) + x].grayscale;
				if(sqaure == 0f){
					grid[x,y].setDecor(roomload (E_Stu, x, y));
					grid [x, y].req.area = AreaType.Studio;
				} else if(sqaure == 0.5f){
					grid[x,y].setDecor(roomload (E_In, x, y));
					grid [x, y].req.area = AreaType.Inside;
				} else {
					grid[x,y].setDecor(roomload (E_Out, x, y));
					grid [x, y].req.area = AreaType.Outside;
				}
				grid [x, y].shortenWalls (true);
			}
		}



		/*for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				grid [x,y] = new Room(roomload (empty, x, y), x, y);
				grid [x, y].shortenWalls (true);
			}
		}



		CreateSpawn ();
		CreateGenerator ();*/
	}

	void CreateSpawn(){
		int cx = Random.Range(Mathf.FloorToInt(mx * 0.5f), mx-1), cy = Random.Range(0, Mathf.FloorToInt(my * 0.5f));

		replace (Office.Spawn, cx, cy);
		grid [cx, cy].chosen = true;
	}

	void CreateGenerator(){
		int cx = Random.Range(Mathf.FloorToInt(mx * 0.5f), mx), cy = Random.Range(Mathf.FloorToInt(my * 0.5f), my);

		replace (Office.Generator, cx, cy);
		grid [cx, cy].chosen = true;
	}

	public Vector2 CreatePlayerSpawn(){
		int cx = Random.Range(0, Mathf.FloorToInt(mx * 0.5f)), cy = Random.Range(0, Mathf.FloorToInt(my * 0.5f));

		replace (pickRoom(Office, cx, cy, Direction.South), cx, cy);
		grid [cx, cy].chosen = true;
		grid [cx, cy].shortenWalls (false);
		lRoom = grid [cx, cy];
		lRoom.manager.inRoom = true;
		return new Vector2 (cx, cy);
	}

	void replace(GameObject newGO, int ox, int oy){
		grid [ox, oy].setDecor(roomload (newGO, ox, oy));
	}

	GameObject roomload(GameObject prefab, int x, int y){
		GameObject c = Instantiate<GameObject> (prefab);
		float h = c.transform.position.y;
		c.transform.parent = this.transform;
		c.transform.position = new Vector3 (x * 14f, h, y * 14f);
		return c;
	}




}

public class Room
{
	private GameObject decor;
	public RoomManager manager;
	public bool chosen = false;
	public Requirements req;
	private int x, y;

	public Room(GameObject decor, int x, int y){
		req = new Requirements ();
		setDecor (decor);
		this.x = x;
		this.y = y;

	}

	public void setDecor(GameObject newGO){
		if(decor != null){
			GameObject.Destroy (decor);
		}
		decor = newGO;
		manager = decor.GetComponent<RoomManager> ();
		shortenWalls (true);
	}

	public bool isDown(){
		if (manager.n != null) {if (manager.n.isWallD ()) {return true;}}
		if (manager.e != null) {if (manager.e.isWallD ()) {return true;}}
		if (manager.s != null) {if (manager.s.isWallD ()) {return true;}}
		if (manager.w != null) {if (manager.w.isWallD ()) {return true;}}
		return false;
	}

	public bool isUp(){
		if (manager.n != null) {if (!manager.n.isWallD ()) {return true;}}
		if (manager.e != null) {if (!manager.e.isWallD ()) {return true;}}
		if (manager.s != null) {if (!manager.s.isWallD ()) {return true;}}
		if (manager.w != null) {if (!manager.w.isWallD ()) {return true;}}
		return false;
	}

	public void shortenWalls(bool ALL_FLAG){
		if(ALL_FLAG){
			if(manager.n != null){manager.n.toggleWall (true);}
			if(manager.w != null){manager.w.toggleWall (true);}
		}
		if (manager.e != null) {
			manager.e.toggleWall (true);
		} 
		if(manager.s != null){manager.s.toggleWall (true);}
	}

	public void heightenWalls(){
		if(manager.n != null){manager.n.toggleWall (false);}
		if(manager.w != null){manager.w.toggleWall (false);}
		if(manager.e != null){manager.e.toggleWall (false);}
		if(manager.s != null){manager.s.toggleWall (false);}
	}

	public bool canMoveDirection(int dx, int dy){
		
		bool[] b = manager.canWalk ();
		if(dx == 1){
			return b [1];
		} else if(dx == -1){
			return b [3];
		} else if(dy == 1){
			return b [0];
		} else if(dy == -1){
			return b [2];
		}
		return false;
	}

}

public enum Direction 
{
	North, South, West, East, NULL
}

public class PlaceRoomInfo
{
	public Room room;
	public Direction dir;

	public PlaceRoomInfo (Room r, Direction d){
		this.room = r;
		this.dir = d;
	}

	public LeaveType getRoomsExitType(){
		LeaveType[] lt = room.manager.getWalk ();
		int ndir = RoomRenderer.getOppDirNum(RoomRenderer.getDirNum(dir));
		return lt [ndir];
	}
}

[System.Serializable]
public class RoomSet 
{
	public List<GameObject> Basic;
	public List<GameObject> Special;
	public GameObject Generator, Spawn;

}

public class Requirements
{
	public AreaType area;
}

public enum AreaType 
{
	Inside, Outside, Empty, Studio, Basement
}
