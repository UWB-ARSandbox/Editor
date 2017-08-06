using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using Photon;
using UnityEngine.UI;

namespace UWBsummercampAPI{

	public class gameManagerBehavior : MonoBehaviour
	{
		// Flags
		protected static bool winFlag = false;
		protected static bool loseFlag = false;
		protected bool timerRunningFlag = false;
		protected bool timerFinishedFlag = false;
		protected bool timerLoopFlag = false;
		protected bool readyToStart = false;
		protected bool firstBufferSync = false;

		// Dynamic Variables
		protected static int points;
		protected static int goalPoints;
		private float timer = 0;
		private int timerMax = 10;
		private bool isGameRunning;

		// Constant Variables
		public static gameManagerBehavior instance;
		static PhotonView pV;//= this.gameObject.AddComponent<PhotonView>();
		static PhotonPlayer pP = new PhotonPlayer(true,0,"PhotonPlayer");
		static networkManagerSummerCamp NetworkManager;
		private Hashtable gameBuffer = new Hashtable();

		private int myTeamID = 0;
		private bool firstUpdate = true;
		private bool updatePlayer = false;
		private bool customPropertyChanged = false;
		private GameObject playerCharacter;
		private string device;
		public Vector3 spawnLocation = new Vector3(0f, 1.32f, -13.64f);

		//Tmp for dev (delete later)
		private CanvasManager mainCanvas;// = new CanvasManager();




		// Use this for initialization
		void Start ()
		{



			pV = gameObject.AddComponent<PhotonView> ();

			pV.viewID = 99999;



			NetworkManager = GameObject.Find ("NetworkManager").GetComponent<networkManagerSummerCamp>();
			myTeamID = NetworkManager.teamID;

			//check with network manager if it's master.
			//if it's, create networked hashtab
			//otherwise, query it and populate local gameBuffer





				goalPoints = NetworkManager.goal;
				updateCache (myTeamID.ToString () + "Points", 0);
				updateCache ("goalPoints", goalPoints);

				points = 0;

			if (!PhotonNetwork.isMasterClient) {
				NetworkManager.joinRoom ();
			} else {

				spawnLocation =  new Vector3(0f, 1.32f, -13.64f);

			}



			instance = this;




			//read or create initial networked database


		}


		// Update is called once per frame
		void Update ()
		{







			if (playerCharacter != null) {

				if (updatePlayer) {
					playerCharacter.GetComponent<coreCharacterBehavior> ().setText (NetworkManager.getPlayerName ());
					mainCanvas = playerCharacter.GetComponentInChildren<CanvasManager> ();
					playerCharacter.GetComponent<coreCharacterBehavior> ().setTeam (myTeamID);
					playerCharacter.GetComponent<coreCharacterBehavior> ().setName (NetworkManager.getPlayerName ());


					updatePlayer = false;
				}
				mainCanvas.refreshDashboard ("myTeam: " + myTeamID + "\n Goal: " + goalPoints + "\n Points: " + points);

			}

			//fix networking:

			if (!firstBufferSync) {
				requestDBsync ();

			} else{

				if (firstUpdate && NetworkManager.isInRoom () && !NetworkManager.HostGame ) {



					//Vector3 spawningPosition;
					Quaternion spawningRotation;


					List<Component> tmpList = new List<Component> ();
					tmpList.Add (this.gameObject.GetComponent<gameManagerBehavior> ());
					pV.ObservedComponents = tmpList;

					//if team doesn't exist this will create point entry for it and avour null exception
					addPoints (0, myTeamID);

					goalPoints = queryCache ("goalPoints");
					points = queryCache (myTeamID.ToString () + "Points");

					firstUpdate = false;





					//For some reason the Camera.main is not working...
					//						Vector3 spawningPosition = Camera.main.gameObject.transform.position;
					//						Quaternion spawningRotation = Camera.main.gameObject.transform.rotation;



					UWBPhotonTransformView[] listOfUWBPhotonOBJ = GameObject.FindObjectsOfType<UWBPhotonTransformView> ();

					foreach (UWBPhotonTransformView UWBPhoton in listOfUWBPhotonOBJ) {

						UWBPhoton.gameObject.GetComponent<PhotonView> ().RPC ("RequestColorRPC", PhotonTargets.MasterClient); 
					}


					switch (NetworkManager.getDevice ()) {
					case "VR":
						print ("VR was choosed!!");

					//spawnLocation = GameObject.Find ("Camera").transform.position;
						spawningRotation = GameObject.Find ("Camera").transform.rotation;
						GameObject.Destroy (GameObject.Find ("Camera"));

						GameObject CameraRig = Instantiate (Resources.Load ("[CameraRig]"), spawnLocation, spawningRotation) as GameObject;
						GameObject ViveAvatar = Instantiate (Resources.Load ("ViveAvatar"), spawnLocation, spawningRotation) as GameObject;
						spawnLocation = new Vector3 (spawnLocation.x + 2f, spawnLocation.y, spawnLocation.z);

						playerCharacter = PhotonNetwork.Instantiate ("DefaultPlayerCharacter", spawnLocation, spawningRotation, 0);
						updatePlayer = true;


						break;
					case "Tango":
						print ("Tango Tango Tango!!!");
						break;
					default:
						print ("Normal PC Stuff");


					//spawningPosition = GameObject.Find ("Camera").transform.position;
						spawningRotation = GameObject.Find ("Camera").transform.rotation;
						playerCharacter = PhotonNetwork.Instantiate ("DefaultPlayerCharacter", spawnLocation, spawningRotation, 0);
						playerCharacter.AddComponent<cameraFollower> ().OnStartFollowing ();
						playerCharacter.GetComponent<coreCharacterBehavior> ().setTeam (myTeamID);
						updatePlayer = true;

						break;
					}




				}
		

			}









			if (points >= goalPoints && readyToStart)
			{
				// winGame();
			}

			// Consume timer event
			timerFinishedFlag = false;

			// Determine if timer is finished
			if (timerRunningFlag)
			{
				timer = timer + Time.deltaTime;

				if (timer > timerMax)
				{
					timerFinishedFlag = true;
					if (timerLoopFlag)
					{
						timer = 0;
					}
					else
					{
						stopTimer();
					}
				}
			}



			//Handle click to move


			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButton(0)))
			{

			//	Debug.Log ("Clicked");
				//declare a variable of RaycastHit struct
				RaycastHit hit;

				//Create a Ray on the tapped / clicked position
				Ray ray = new Ray();

				//for unity editor
				#if (UNITY_EDITOR || UNITY_STANDALONE)
				ray =  GameObject.Find("Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				//for touch device
				#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
				ray =  GameObject.Find("Camera").GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
				#endif

				//Check if the ray hits any collider
				if (Physics.Raycast(ray, out hit))
				{
					Vector3 destPoint; 
					//save the click / tap position
					destPoint = hit.point;
					//as we do not want to change the y axis value based on touch position, reset it to original y axis value
					destPoint.y = playerCharacter.transform.position.y - playerCharacter.transform.localScale.y ;

					playerCharacter.GetComponent<coreCharacterBehavior> ().setClickToMove (destPoint);
				}

			}



		}

		#region Win/Lose Game
		public void winGame()
		{
			if (winFlag || loseFlag)
				return;

			if (pV != null  && NetworkManager.isInRoom() )
			{
				// This file PunRPC
				pV.RPC("winGameRPC", PhotonTargets.All); // *
			}
			else
			{
				// Make this character win
				winGameRPC();
			}
		}

		[PunRPC]
		public void winGameRPC()
		{
			if (!winFlag && !loseFlag)
			{
				winFlag = true;
				Instantiate(Resources.Load("WinCanvas"));
			}
		}

		public void loseGame()
		{
			if (winFlag || loseFlag)
				return;

			if (pV != null  && NetworkManager.isInRoom() )
			{
				// This file PunRPC
				pV.RPC("loseGameRPC", PhotonTargets.All); // *
			}
			else
			{
				// Make this character win
				loseGameRPC();
			}
		}

		[PunRPC]
		public void loseGameRPC()
		{
			if (!winFlag && !loseFlag)
			{
				loseFlag = true;
				Instantiate(Resources.Load("LoseCanvas"));
			}
		}
		#endregion

		#region Points
		public void addPoints(int pointsTMP = 10, int teamID = 0)
		{


			if (pV != null  && NetworkManager.isInRoom() )
			{
				// This file PunRPC
				pV.RPC("addPointsRPC", PhotonTargets.All, pointsTMP, teamID); // *
			}
			else
			{
				addPointsRPC(pointsTMP, teamID);
			}

		
		
		
		}


		[PunRPC]
		public void addPointsRPC(int pointsAdded, int teamID)
		{


			//update locally
			if (teamID == myTeamID) {/*

				string key = teamID.ToString() ;
				key = key + "Points";
				try{
					points = pointsTMP + (int) gameBuffer[key];

				}
				catch (System.Exception e)
				{
					Debug.Log ("cache didn't exist yet...");
					points = pointsTMP;
				}

				points = points + pointsTMP;


*/
				points = points + pointsAdded;
			}






			if (PhotonNetwork.isMasterClient ) {


				int totalPoints;

				string key = teamID.ToString() ;
				key = key + "Points";
				try{
					totalPoints = pointsAdded + (int) gameBuffer[key];

				}
				catch (System.Exception e)
				{
					Debug.Log ("cache didn't exist yet...");
					totalPoints = pointsAdded;
				}

				updateCache (teamID.ToString () + "Points", totalPoints);

			}

			Debug.Log("addpoints to "+teamID);
			//  points = pointTotal;
		}

		#endregion






		#region Custom Memory Management

		public bool requestDBsync()
		{

			if (pV != null  && NetworkManager.isInRoom()  )
			{

				pV.RPC ("syncDBRPC", PhotonTargets.All); // *
				return true;
			}


			return false;

		}


		[PunRPC]
		public bool syncDBRPC()
		{

			if (pV != null  && NetworkManager.isInRoom() )
			{


				if (PhotonNetwork.isMasterClient) {
					// This file PunRPC
					pV.RPC ("updateDBRPC", PhotonTargets.All, gameBuffer, spawnLocation.x, spawnLocation.y, spawnLocation.z ); // *
					return true;
				}


			}


			return false;
		}


		[PunRPC]
		public void updateDBRPC(Hashtable bufferTMP, float x, float y , float z)
		{
			if (!PhotonNetwork.isMasterClient) {
				spawnLocation = new Vector3 (x, y, z);
				gameBuffer = bufferTMP;
				firstBufferSync = true;
			//	Debug.Log ("buffer updated " + gameBuffer);
			}
		}




		#endregion




		#region Goals


		public void setGoal(int goalTMP = 10)
		{
			int newGoal = goalTMP;

			if (pV != null  && NetworkManager.isInRoom() )
			{
				// This file PunRPC
				pV.RPC("setGoalRPC", PhotonTargets.All, newGoal); // *
			}
			else
			{
				setGoalRPC(newGoal);
			}
		}


		[PunRPC]
		public void setGoalRPC(int newGoal)
		{
			//Debug.Log("new Goal Set: "+newGoal);

			//update local variable and networked cache
			goalPoints = newGoal;
			updateCache ("goalPoints", newGoal);


		}

		#endregion


		#region Team
		public int getTeam(){


			return myTeamID;

		}

		public GameObject getPlayer(){
			return playerCharacter;
		}

		#endregion





		#region Location
		public void setMainSpawnLocation( Vector3 newSpawnLocation){

			spawnLocation = newSpawnLocation;

		}



		#endregion



		#region Timer
		/* Should these be networked? There will be serious lag if they are. */
		public void setTimer(int timerLength, bool makeRepeat = false)
		{
			timerMax = timerLength;
			makeTimerRepeat(makeRepeat);
		}

		public void makeTimerRepeat(bool makeRepeat)
		{
			timerLoopFlag = makeRepeat;
		}

		public void startTimer()
		{
			Debug.Log("Start Timer");
			timerRunningFlag = true;
		}

		public void stopTimer()
		{
			timerRunningFlag = false;
		}

		public bool timeIsUp()
		{
			// Event is consumed in update function
			return timerFinishedFlag;
		}

		public bool timerIsRunning()
		{
			return timerRunningFlag;
		}
		#endregion





		#region Network Memory Management


		private void updateCache(string key, int value){


		//	Debug.Log ("updateCache key:" + key + " value: "+value.ToString());
			gameBuffer[key] = value;
			updateNetworkedCache ();



		}


		private int queryCache(string key){


			return (int) gameBuffer[key] ;
		}



		private void updateNetworkedCache (){

			//custom properties not really working...
			//pP.SetCustomProperties(gameBuffer, gameBuffer);

			//this is the workarouns custom properties issue.
			syncDBRPC ();



		}

		//updates everytime customproperties are update on the network
		//this force us to have a fresh local copy at every change 
		public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
		{

			customPropertyChanged = true;
			PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
			gameBuffer = playerAndUpdatedProps[1] as Hashtable;

		}

		#endregion

	}
}





























