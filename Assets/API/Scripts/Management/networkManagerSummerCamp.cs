using System;
using Photon;
using UnityEngine;
using UWBsummercampAPI;



public class networkManagerSummerCamp : PunBehaviour {

	public bool HostGame = true;
	public bool privateRoom = true;
	public string RoomName = "UWBSummerCamp";
	public int teamID = 0;
	public int goal = 99999;
	private Room currentRoom;
	private RoomOptions roomOptions;
	private SceneBuffer sceneBuffer;
	private string playerName;
	private string device = "Workstation";
	private gameManagerBehavior GameManager;


	// Use this for initialization
	void Start()
	{
		
		GameObject _GameManager = GameObject.Find("GameManager");
		if (_GameManager == null && (GameObject.Find("LobbyManager") == null )  ){
			_GameManager = new GameObject ();
			_GameManager.name = "GameManager";
			_GameManager.AddComponent<gameManagerBehavior> ();
		}

		if (_GameManager != null) {
			GameManager = _GameManager.GetComponent<gameManagerBehavior> ();
		}


	//	PhotonNetwork.automaticallySyncScene = true;

		roomOptions = new RoomOptions ();
		roomOptions.maxPlayers = 5;
		roomOptions.isOpen = true;
		roomOptions.isVisible = !privateRoom;

		try{
		sceneBuffer = GameObject.Find ("SceneBuffer").GetComponent<SceneBuffer>();
		} catch {

		}

		if (sceneBuffer != null) {

			string[] bufferTMP = sceneBuffer.getBuffer ();


			RoomName =	 bufferTMP[0];
			teamID = int.Parse(bufferTMP[1]);
			playerName = bufferTMP [2];
			device = bufferTMP [3];
			HostGame = false;


		} 

		if (!PhotonNetwork.connected) {
//			PhotonNetwork.ConnectToMaster ("172.21.209.145", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 

			PhotonNetwork.ConnectToMaster ("10.156.32.153", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1");
		}







	
	}


	public bool isInRoom(){

		return PhotonNetwork.inRoom;

	}


	public Room getRoom(){

		return 	PhotonNetwork.room;
	}

	public String getDevice(){

		return 	device;
	}


	public String getPlayerName(){

		return 	playerName;
	}


	public override void OnConnectedToMaster()
	{


		Debug.Log("OnConnectedToMaster() was called by PUN");



		if (HostGame) {
			PhotonNetwork.CreateRoom (RoomName,roomOptions, TypedLobby.Default);
		} else {
			PhotonNetwork.JoinRoom (RoomName);
		}

	}


	public override void OnDisconnectedFromPhoton()
	{


		Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");        
	}

	public bool joinRoom(){
		

			if (PhotonNetwork.connectionState.ToString () == "Disconnected") {
			//	PhotonNetwork.ConnectToMaster ("172.21.209.145", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 
			PhotonNetwork.ConnectToMaster ("10.156.32.153", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 
			}		

		if (!HostGame) {
			PhotonNetwork.JoinRoom (RoomName);
			return true;
		}


		return false;

	}

}
