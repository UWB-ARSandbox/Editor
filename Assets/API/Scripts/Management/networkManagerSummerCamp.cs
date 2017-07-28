using System;
using Photon;
using UnityEngine;
//using UnityEditor;

public class networkManagerSummerCamp : PunBehaviour {

	public bool HostGame = false;
	public string RoomName = "UWBSummerCamp";


	// Use this for initialization
	void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
//		PhotonNetwork.ConnectUsingSettings("1");
		PhotonNetwork.ConnectToMaster ("172.21.209.145", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 
	
	}








	public override void OnConnectedToMaster()
	{


		Debug.Log("OnConnectedToMaster() was called by PUN");

		if (HostGame) {
			PhotonNetwork.CreateRoom (RoomName);
		} else {
			PhotonNetwork.JoinRoom (RoomName);
		}

	}


	public override void OnDisconnectedFromPhoton()
	{


		Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");        
	}

}
