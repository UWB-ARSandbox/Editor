using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class coreCharacterBehavior : MonoBehaviour
{
    // Enums
	protected enum sizes {small, normal, big};

    // Flags
    protected bool isMovingFlag = false;
    protected bool isJumpingFlag = false;

	public int points;

	protected int goalPoints;

	protected int speed;

    // Static Variables
    PhotonView pV;

    // Dynamic Variables
	protected sizes characterSize = sizes.normal;

    // Use this for initialization
    void Start ()
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
	void Update ()
    {
        MethodInfo method = this.GetType().GetMethod("updateGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }

		if (points >= goalPoints) {
			winGame ();
		}


		if (isMovingFlag) {
			transform.Translate(Vector3.forward * Time.deltaTime * speed);

		}
    }



	protected void moveForward( int speedTMP = 5)
    {
		speed = speedTMP;
		isMovingFlag = true;
	}


	protected void stopMoving()
	{

		isMovingFlag = false;
	}

    public bool makeSmaller( )
    {
        if (characterSize == sizes.small)
        {
			return false;
		}

        // This file PunRPC
        pV.RPC("makeSmallerRPC", PhotonTargets.All);

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

        // This file PunRPC
        pV.RPC("makeBiggerRPC", PhotonTargets.All);

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



	public void addPoints( int pointsTMP = 10)
	{
		Debug.Log("addpoints!!");
		points = points + pointsTMP;
	}


	protected void winGame() {
			stopMoving ();
			Instantiate(Resources.Load("WinCanvas"));
	}



	protected void loseGame() {

		stopMoving ();
			Instantiate(Resources.Load("LoseCanvas"));
	}

	protected void setGoal(int goalPointsTMP){

		goalPoints = goalPointsTMP;

	}



}
