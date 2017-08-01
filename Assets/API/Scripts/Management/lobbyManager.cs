using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lobbyManager : MonoBehaviour {

	public Dropdown dropdownRoom;
	public Dropdown dropdownTeam;
	public InputField playerInputfield;
	public Button joinButtom;


	private string roomName; 
	private int teamID;
	private string playerName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



		roomName = dropdownRoom.onValueChanged.ToString ();


		
	}
}
