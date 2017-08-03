using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class lobbyManager : PunBehaviour {

	public Dropdown dropdownRoom;
	public Dropdown dropdownTeam;
	public Dropdown dropdownDevices;
	public InputField playerInputfield;
	public Button joinButton;
	public Button refreshRoomListButton;


	private string roomName; 
	private int teamID;
	private string playerName;
	private string device;


	private int[] teamsList = {1, 2, 3, 4};
	private string[] deviceList = { "Workstation", "VR" };

	// Use this for initialization
	void Start () {
		
		PhotonNetwork.ConnectToMaster ("172.21.209.145", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 

		#if (UNITY_ANDROID)

		deviceList = new string[] { "Tango"};

		#endif


		//Setup Interface

		//Setup Rooms
		refreshRoomList();


		//Set Buttom Listner
		refreshRoomListButton.onClick.AddListener(refreshRoomList);

		//Set Buttom Listener
		joinButton.onClick.AddListener(joinChoosedGame);



		//Setup Teams
		dropdownTeam.options.Clear ();
		foreach (int team in teamsList) 
		{
			dropdownTeam.options.Add (new Dropdown.OptionData(team.ToString()));

		}
		dropdownTeam.RefreshShownValue ();



		//Setup Devices
		dropdownDevices.options.Clear ();
		foreach (string device in deviceList) 
		{
			dropdownDevices.options.Add (new Dropdown.OptionData(device));

		}
		dropdownDevices.RefreshShownValue ();



	}
	
	// Update is called once per frame
	void Update () {


		//update values
		try{
		roomName = dropdownRoom.options[ dropdownRoom.value].text;
		teamID = int.Parse( dropdownTeam.options[dropdownTeam.value].text);
		playerName = playerInputfield.text;
		device = dropdownDevices.options[dropdownDevices.value].text;
		
		}
		catch{

		}

		if (roomName == null || playerName == "") {
			joinButton.interactable = false;
		} else {
			joinButton.interactable = true;
		}

		
	}



	private void refreshRoomList(){

		RoomInfo[] roomList = PhotonNetwork.GetRoomList ();


		dropdownRoom.options.Clear ();

		foreach (RoomInfo room in roomList) 
		{
			dropdownRoom.options.Add (new Dropdown.OptionData(room.name));
			Debug.Log (room.name);
		}

		dropdownRoom.RefreshShownValue ();

	}




	private void joinChoosedGame()
	{

		GameObject SceneBufferOBJ = new GameObject();
		SceneBufferOBJ.name = "SceneBuffer";
		SceneBuffer sceneBuffer = SceneBufferOBJ.AddComponent<SceneBuffer> ();


		sceneBuffer.updateBuffer (roomName, teamID, playerName, device);
		Object.DontDestroyOnLoad (SceneBufferOBJ);

		Application.LoadLevel ("TemplateScene");


	}






	public override void OnConnectedToMaster()
	{


		Debug.Log("OnConnectedToMaster() was called by PUN");


		PhotonNetwork.JoinLobby (); 






	}



}
