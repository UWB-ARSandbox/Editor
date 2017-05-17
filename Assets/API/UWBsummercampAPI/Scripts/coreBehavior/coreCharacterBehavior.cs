using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class coreCharacterBehavior : MonoBehaviour
{
    // Enums
    protected enum sizes { small, normal, big };

    // Flags
    protected bool isMovingFlag = false;
    protected bool isJumpingFlag = false;
    protected bool winFlag = false;
    protected bool loseFlag = false;

    public int points;

    protected int goalPoints;

    protected int speed;

    // Static Variables
    PhotonView pV;

    // Dynamic Variables
    protected sizes characterSize = sizes.normal;

    // Use this for initialization
    void Start()
    {
        points = 0;

        goalPoints = 999999;

        pV = transform.GetComponent<PhotonView>();

        MethodInfo method = this.GetType().GetMethod("buildGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MethodInfo method = this.GetType().GetMethod("updateGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }

        if (points >= goalPoints)
        {
            winGame();
        }


        if (isMovingFlag)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

        }
    }

    protected void moveForward(int speedTMP = 5)
    {
        speed = speedTMP;
        isMovingFlag = true;
    }


    protected void stopMoving()
    {

        isMovingFlag = false;
    }

    public bool makeSmaller()
    {
        if (characterSize == sizes.small)
        {
            return false;
        }

        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeSmallerRPC", PhotonTargets.All);
        }
        else
        {
            // Make this character smaller without calling
            //  an RPC; this is unnetworked
            makeSmallerRPC();
        }

        return true;
    }

    [PunRPC]
    public void makeSmallerRPC()
    {
        CapsuleCollider m_Capsule = GetComponent<CapsuleCollider>();
        if (characterSize == sizes.normal)
        {
            transform.localScale = new Vector3(.3f, .3f, .3f);
            characterSize = sizes.small;
            m_Capsule.height = m_Capsule.height / 3f;
            m_Capsule.center = m_Capsule.center / 3f;

        }
        else if (characterSize == sizes.big)
        {
            transform.localScale = new Vector3(1, 1, 1);
            characterSize = sizes.normal;
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
        }
    }

    public bool makeBigger()
    {

        if (characterSize == sizes.big)
        {
            return false;
        }

        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeBiggerRPC", PhotonTargets.All);
        }
        else
        {
            // Make this character bigger without calling
            //  an RPC; this is unnetworked
            makeBiggerRPC();
        }

        return true;
    }

    [PunRPC]
    public void makeBiggerRPC()
    {
        CapsuleCollider m_Capsule = GetComponent<CapsuleCollider>();
        if (characterSize == sizes.normal)
        {
            transform.localScale = new Vector3(2, 2, 2);
            characterSize = sizes.big;

            m_Capsule.height = m_Capsule.height * 2f;
            m_Capsule.center = m_Capsule.center * 2f;

        }
        else if (characterSize == sizes.small)
        {
            transform.localScale = new Vector3(1, 1, 1);
            characterSize = sizes.normal;

            m_Capsule.height = m_Capsule.height * 3f;
            m_Capsule.center = m_Capsule.center * 3f;
        }
    }

    public bool isMoving()
    {
        return isMovingFlag;
    }

    public void jump(int forceTMP = 10)
    {
        int force = forceTMP * 50;
        GetComponent<Rigidbody>().AddForce(0, force, 0);
    }


    public bool isJumping()
    {
        return isJumpingFlag;
    }

    #region GameManager Methods
    /* =========================================== */
    /* THESE METHODS SHOULD MOVE TO A GAME MANAGER */

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

    public void winGame()
    {
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
            stopMoving();
            Instantiate(Resources.Load("WinCanvas"));
        }
    }

    public void loseGame()
    {
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
            stopMoving();
            Instantiate(Resources.Load("LoseCanvas"));
        }
    }

    // Does this need to be networked, or will we allow the server to dictate the win conditions,
    //  and therefore just let the server call winGame?
    // Will we need the goal for any kind of counter?
    // Should students program in the goal on their own?
    protected void setGoal(int goalPointsTMP)
    {
        goalPoints = goalPointsTMP;
    }

    /* =========================================== */
    #endregion
}
