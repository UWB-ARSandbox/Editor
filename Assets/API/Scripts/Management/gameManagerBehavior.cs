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
	private bool readyToStart = false;
	private bool customPropertyChanged = false;
	private GameObject playerCharacter;


	//Tmp for dev (delete later)
	private Text canvasText ;




    // Use this for initialization
    void Start ()
    {
			

			//Tmp for dev (delete later)
			try{
			canvasText = GameObject.Find("Canvas").GetComponentInChildren<Text> ();
			}
			catch{
				
				GameObject NewCanvas = new GameObject();
				canvasText = NewCanvas.AddComponent<Text>();

			}

			//Tmp for dev finished


			pV = gameObject.AddComponent<PhotonView> ();

			pV.viewID = 99999;
		


			NetworkManager = GameObject.Find ("NetworkManager").GetComponent<networkManagerSummerCamp>();
			myTeamID = NetworkManager.teamID;

			//check with network manager if it's master.
			//if it's, create networked hashtab
			//otherwise, query it and populate local gameBuffer



			if (NetworkManager.HostGame) {

				goalPoints = NetworkManager.goal;
				updateCache (myTeamID.ToString () + "Points", 0);
				updateCache ("goalPoints", goalPoints);

				points = 0;
				readyToStart = true;

			} 



		
        
        instance = this;




//read or create initial networked database


    }
	

	// Update is called once per frame
	void Update ()
		{




			//Tmp for dev (delete later)


			canvasText.text = "";
			if (Input.GetKeyDown (KeyCode.Space)) {
				//setGoal (22);

				Debug.Log ("BUFFEERRR RAW: ");
				Debug.Log (pP.customProperties.ToString ());
				Debug.Log ("BUFFEERRR: ");
				Debug.Log (gameBuffer);

			}

			canvasText.text = "myTeam: " + myTeamID + "\n Goal: " + goalPoints + "\n Points: " + points;

			//Tmp for dev finished



			//fix networking:
			if (firstUpdate && NetworkManager.isInRoom()) {
				requestDBsync ();

				if (readyToStart) {

					List<Component> tmpList = new List<Component> ();
					tmpList.Add (this.gameObject.GetComponent<gameManagerBehavior> ());
					pV.ObservedComponents = tmpList;

					//if team doesn't exist this will create point entry for it and avour null exception
					addPoints (0, myTeamID);

					goalPoints = queryCache ("goalPoints");
					points = queryCache (myTeamID.ToString () + "Points");

					firstUpdate = false;

					if (! NetworkManager.HostGame) {
						//photon instantiate and place in playerCharacter

						Vector3 spawningPosition = Camera.main.gameObject.transform.position;
						Quaternion spawningRotation = Camera.main.gameObject.transform.rotation;
						playerCharacter = PhotonNetwork.Instantiate ("DefaultPlayerCharacter", spawningPosition, spawningRotation, 0);


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

				Debug.Log ("Clicked");
				//declare a variable of RaycastHit struct
				RaycastHit hit;

				//Create a Ray on the tapped / clicked position
				Ray ray = new Ray();

				//for unity editor
				#if (UNITY_EDITOR || UNITY_STANDALONE)
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				//for touch device
				#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
				ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				#endif

				//Check if the ray hits any collider
				if (Physics.Raycast(ray, out hit))
				{
					Vector3 destPoint; 
					//save the click / tap position
					destPoint = hit.point;
					//as we do not want to change the y axis value based on touch position, reset it to original y axis value
					destPoint.y = playerCharacter.transform.position.y - playerCharacter.transform.localScale.y;
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
			int totalPoints = 0;
			try{
			totalPoints = pointsTMP + queryCache (NetworkManager.teamID.ToString () + "Points");
			}
			catch (System.Exception e)
			{

			}
			updateCache (teamID.ToString () + "Points", totalPoints);

        if (pV != null  && NetworkManager.isInRoom() )
        {
            // This file PunRPC
				pV.RPC("setPointsRPC", PhotonTargets.All, totalPoints, teamID); // *
        }
        else
        {
				setPointsRPC(totalPoints, teamID);
        }
    }


    [PunRPC]
		public void setPointsRPC(int pointTotal, int teamID)
    {
			if (teamID == myTeamID) {
				points = pointTotal;
			}

			if (NetworkManager.HostGame) {


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


				if (NetworkManager.HostGame) {
					// This file PunRPC
					pV.RPC ("updateDBRPC", PhotonTargets.All, gameBuffer); // *
					return true;
				}


			}


			return false;
		}


		[PunRPC]
		public void updateDBRPC(Hashtable bufferTMP)
		{
			if (!NetworkManager.HostGame) {
				gameBuffer = bufferTMP;
				readyToStart = true;
				Debug.Log ("buffer updated " + gameBuffer);
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
			Debug.Log("new Goal Set: "+newGoal);

			//update local variable and networked cache
			goalPoints = newGoal;
			updateCache ("goalPoints", newGoal);


		}

		#endregion


		#region Team
		public int getTeam(){


			return myTeamID;

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