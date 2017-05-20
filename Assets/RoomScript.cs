using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomScript : MonoBehaviour
{
	public virtual void OnEnter(){
	}

	public virtual void OnLeave(){
	}

	public virtual void OnAction(){
		Debug.Log ("Old Action");
	}
}

