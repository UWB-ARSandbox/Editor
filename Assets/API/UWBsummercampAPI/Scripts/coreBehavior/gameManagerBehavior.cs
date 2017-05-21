using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Constant Variables
    public static gameManagerBehavior instance;
    static PhotonView pV;

    // Use this for initialization
    void Start ()
    {
        points = 0;
        goalPoints = 999999;
        
        instance = this;
        pV = transform.GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (points >= goalPoints)
        {
            winGame();
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
    public void addPoints(int pointsTMP = 10)
    {
        int totalPoints = points + pointsTMP;

        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("setPointsRPC", PhotonTargets.All, totalPoints); // *
        }
        else
        {
            setPointsRPC(totalPoints);
        }
    }


    [PunRPC]
    public void setPointsRPC(int pointTotal)
    {
        Debug.Log("addpoints!!");
        points = pointTotal;
    }

    // Does this need to be networked, or will we allow the server to dictate the win conditions,
    //  and therefore just let the server call winGame?
    // Will we need the goal for any kind of counter?
    // Should students program in the goal on their own?
    public void setGoal(int goalPointsTMP)
    {
        goalPoints = goalPointsTMP;
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
}
