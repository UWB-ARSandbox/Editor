using System;
using Photon;
using UnityEngine;
//using UnityEditor;

public class networkManagerSummerCamp : PunBehaviour {



	// Use this for initialization
	void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.ConnectUsingSettings("1");
	
	}








	public override void OnConnectedToMaster()
	{


		Debug.Log("OnConnectedToMaster() was called by PUN");
		PhotonNetwork.CreateRoom ("SummerCampTest");

	}


	public override void OnDisconnectedFromPhoton()
	{


		Debug.LogWarning("OnDisconnectedFromPhoton() was called by PUN");        
	}

}
