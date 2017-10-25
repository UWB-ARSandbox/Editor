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
	public Toggle privateRoomToggle;
	public InputField roomNameInputField;
	public InputField serverIPInputField;
	public Toggle customServerToggle;

	private string roomName; 
	private int teamID;
	private string playerName;
	private string device;
	private bool privateRoom; 
	private bool validRoom;
	private string serverIP;


	private string[] teamsList = {"1(Red)", "2(Purple)", "3(Blue)", "4(Yellow)"};
	private string[] deviceList = { "Workstation", "VR" };

	// Use this for initialization
	void Start () {
		
		//PhotonNetwork.ConnectToMaster ("172.21.209.145", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 
		//PhotonNetwork.ConnectToMaster ("10.156.32.153", 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 
		#if (UNITY_ANDROID)

		deviceList = new string[] { "Tango"};

		#endif


		//Setup Interface

		//Setup Rooms
		privateRoom = false; //this variable seems useless and should be revised to remove
		privateRoomToggle.isOn = false;

		customServerToggle.isOn = false;
		serverIPInputField.text = "172.21.209.145";
		serverIP = "172.21.209.145";
		serverIPInputField.interactable = false;

		//refreshRoomList();


		//Set Listners
		refreshRoomListButton.onClick.AddListener(refreshRoomList);
		joinButton.onClick.AddListener(joinChoosedGame);
		privateRoomToggle.onValueChanged.AddListener (manageRoomPrivacy);
		roomNameInputField.onValueChanged.AddListener(checkRoomName);
		dropdownRoom.onValueChanged.AddListener (updateRoomNameDrop);
		serverIPInputField.onValueChanged.AddListener(updateServerIP);
		customServerToggle.onValueChanged.AddListener (manageCustomServer);


		//Setup Teams
		dropdownTeam.options.Clear ();
		foreach (string team in teamsList) 
		{
			dropdownTeam.options.Add (new Dropdown.OptionData(team));

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

			teamID =  dropdownTeam.value +1 ; 
			playerName = playerInputfield.text;
			device = dropdownDevices.options[dropdownDevices.value].text;


		}
		catch{

		}

		if (roomName == null || playerName == "" || !validRoom) {
			joinButton.interactable = false;
		} else {
			joinButton.interactable = true;
		}

		
	}



	private void refreshRoomList(){



		if (PhotonNetwork.connected) {

			PhotonNetwork.Disconnect ();
		}
		PhotonNetwork.ConnectToMaster (serverIP, 4530, "6bb09fb9-6bbc-4a7d-a181-44797df0c001", "1"); 


		RoomInfo[] roomList = PhotonNetwork.GetRoomList ();


		dropdownRoom.options.Clear ();

		foreach (RoomInfo room in roomList) 
		{
			dropdownRoom.options.Add (new Dropdown.OptionData(room.name));
		}

		if (roomList.Length >0){
		updateRoomNameDrop (0);
			validRoom = true;
		}
		dropdownRoom.RefreshShownValue ();



	}





	private void updateRoomNameDrop(int roomNameIdx){

		roomName = dropdownRoom.options[roomNameIdx].text;
		Debug.Log ("room changed to: " + roomName);

	}


	private void updateServerIP(string tmpRoomName){
		Debug.Log ("IP value changed");
		serverIP = tmpRoomName;

	}

	private void checkRoomName(string tmpRoomName){
		//need to do a beter validatiom!!! like the following
		/*

		if (tmpRoomName != "") {
			//need to check if the room name is valid...

			if(PhotonNetwork.JoinRoom (roomName)){

				PhotonNetwork.LeaveRoom();
				validRoom = true;
				roomName = tmpRoomName;
			}
				} else {

					validRoom = false;
				}

*/

		if (tmpRoomName != "") {
				validRoom = true;
				roomName = tmpRoomName;

		} else {

			validRoom = false;
		}



		}



	private void manageCustomServer(bool customServer)
	{

		if (customServer) {
			serverIPInputField.text = "";
			serverIP = "localhost";
			serverIPInputField.interactable = true;
			return;

		}

		serverIPInputField.text = "172.21.209.145";
		serverIPInputField.interactable = false;


	}


	private void manageRoomPrivacy(bool privateRoom)
	{


		if (privateRoom){
			dropdownRoom.gameObject.SetActive(false);
			roomNameInputField.gameObject.SetActive(true);
			roomNameInputField.text = "";
			roomName = roomNameInputField.text;
			validRoom = false;

		} else{
			dropdownRoom.gameObject.SetActive(true);
			roomNameInputField.gameObject.SetActive(false);	
			validRoom = true;
		}


	}



	private void joinChoosedGame()
	{

		GameObject SceneBufferOBJ = new GameObject();
		SceneBufferOBJ.name = "SceneBuffer";
		SceneBuffer sceneBuffer = SceneBufferOBJ.AddComponent<SceneBuffer> ();


		sceneBuffer.updateBuffer (roomName, teamID, playerName, device, serverIP);
		Object.DontDestroyOnLoad (SceneBufferOBJ);

		Application.LoadLevel ("TemplateScene");


	}






	public override void OnConnectedToMaster()
	{


		Debug.Log("OnConnectedToMaster() was called by PUN");


		PhotonNetwork.JoinLobby (); 






	}



}
