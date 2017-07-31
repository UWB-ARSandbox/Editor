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
	static PhotonPlayer pP = new PhotonPlayer(true,0,"localCOmputer");
	static networkManagerSummerCamp NetworkManager;
	private Hashtable gameBuffer = new Hashtable();
	
	private int myTeamID = 0;
	private bool firstUpdate = true;
	private bool customPropertyChanged = false;



		//Tmp for dev (delete later)
		private Text canvasText ;




    // Use this for initialization
    void Start ()
    {
			

			//Tmp for dev (delete later)

			canvasText = GameObject.Find("Canvas").GetComponentInChildren<Text> ();


			//Tmp for dev finished


			pV = gameObject.AddComponent<PhotonView> ();

			pV.viewID = 99999;
		


			NetworkManager = GameObject.Find ("NetworkManager").GetComponent<networkManagerSummerCamp>();
			myTeamID = NetworkManager.teamID;

			//check with network manager if it's master.
			//if it, so create networked hashtab
			//otherwise, wuery it and populate local gameBuffer



			if (NetworkManager.HostGame) {
				goalPoints = NetworkManager.goal;
				updateCache (myTeamID.ToString () + "Points", 0);
				updateCache ("goalPoints", goalPoints);
				updateCache ("Inicialized", 1);

				points = 0;



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
				setGoal (22);
				Debug.Log ("BUFFEERRR RAW: ");
				Debug.Log (pP.customProperties.ToString());
				Debug.Log ("BUFFEERRR: ");
				Debug.Log (gameBuffer);
			}

			canvasText.text = "myTeam: "+myTeamID+"\n Goal: "+goalPoints+"\n Points: "+points;

			//Tmp for dev finished



			//fix networking:
			if (firstUpdate && customPropertyChanged) {
				List<Component> tmpList = new List<Component>();
				tmpList.Add (this.gameObject.GetComponent<gameManagerBehavior> ());
				pV.ObservedComponents = tmpList;


				if (!NetworkManager.HostGame) {
					gameBuffer = pP.customProperties;

					Debug.Log ("BUFFEERRR RAW: ");
					Debug.Log (pP.customProperties.ToString());
					Debug.Log ("BUFFEERRR: ");
					Debug.Log (gameBuffer);
					//				goalPoints = queryCache ("goalPoints");
					//				points = queryCache (myTeamID.ToString () + "Points");


				}


				firstUpdate = false;
			}

        if (points >= goalPoints)
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
    }

    #region Win/Lose Game
    public void winGame()
    {
        if (winFlag || loseFlag)
            return;

        if (pV != null)
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

        if (pV != null)
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
			int totalPoints = pointsTMP + queryCache (NetworkManager.teamID.ToString () + "Points");
			updateCache (teamID.ToString () + "Points", totalPoints);

        if (pV != null)
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


			Debug.Log("addpoints to "+teamID);
      //  points = pointTotal;
    }

    #endregion



		#region Goals


		public void setGoal(int goalTMP = 10)
		{
			int newGoal = goalTMP;

			if (pV != null)
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


			pP.SetCustomProperties(gameBuffer, gameBuffer);



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