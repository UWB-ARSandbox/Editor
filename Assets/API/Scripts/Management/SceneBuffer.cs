using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBuffer : MonoBehaviour {

	private string roomName; 
	private int teamID;
	private string playerName;
	private string device;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateBuffer(string _roomName, int _teamID , string _playerName, string _device){
		roomName = _roomName;
		teamID = _teamID;
		playerName = _playerName;
		device = _device;
	}

	public string[] getBuffer(){
		string[] currentBuffer = { roomName, teamID.ToString (), playerName, device };

		return currentBuffer;
	}
}
