using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class coreObjectsBehavior : MonoBehaviour
{
	protected bool playerIsTouchingFlag = false;
	protected bool touched = false;
	private GameObject iteractionObj = null;
    private Color netColor;

	private bool shouldSpin = false;

	// Static Variables
	PhotonView pV;

	// Use this for initialization
	void Start ()
	{
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



		if (shouldSpin)
        {
			transform.Rotate(0,20*Time.deltaTime,0);
		}


	}






    // Use this for initialization
    private void Awake()
    {
        netColor = this.gameObject.GetComponent<Renderer>().material.color;
    }

    protected bool playerIsTouching()
    {
        // Consume touching event? We consume
        //  it in every other event
        bool retVal = playerIsTouchingFlag;
        playerIsTouchingFlag = false;
        return retVal;
    }


	void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with [" + collision.gameObject.name + "]");
		if (collision.gameObject.GetComponent<coreCharacterBehavior> () != null)
        {
			iteractionObj = collision.gameObject;
            playerIsTouchingFlag = true;
        }
	}
		


	protected bool turnPlayerLeft()
    {

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Turn Left: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("Left");

            // Shave off excess rotation to (TRY to) ensure moving in a cardinal direction
            double newY = Math.Round(iteractionObj.transform.rotation.eulerAngles.y / 90);
            newY = ((int)newY - 1) % 4;

            iteractionObj.transform.rotation = Quaternion.Euler(0, (int)newY * 90, 0);
			iteractionObj = null;
			return true;
		}

	}


	protected bool turnPlayerRight()
    {

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Turn Right: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("Right");

            // Shave off excess rotation to (TRY to) ensure moving in a cardinal direction
            double newY = Math.Round(iteractionObj.transform.rotation.eulerAngles.y / 90);
            newY = ((int)newY + 1) % 4;

            iteractionObj.transform.rotation = Quaternion.Euler(0, (int)newY * 90, 0);
            iteractionObj = null;
            return true;
		}

	}


	protected bool turnPlayerBackwards()
    {

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Turn Right: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("Around");

			iteractionObj.transform.Rotate (new Vector3 (iteractionObj.transform.rotation.x, 180, iteractionObj.transform.rotation.z));
			iteractionObj = null;
			return true;
		}

	}






	protected bool jumpPlayer(int forceTMP = 10 )
    {


		int force = forceTMP *50;

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Jump: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("jump");
            iteractionObj.GetComponent<coreCharacterBehavior>().jump();
            iteractionObj = null;
			return true;
		}

	}



	protected bool makePlayerSmaller( )
    {

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Make Player Smaller: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("MakePlayerSmaller()");
			iteractionObj.GetComponent<coreCharacterBehavior>().makeSmaller();
			iteractionObj = null;
			return true;
		}

	}


	protected bool givePoints( int points = 10 ){

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot give points: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("PointsGiven");
			iteractionObj.GetComponent<coreCharacterBehavior>().addPoints(points);
			iteractionObj = null;
			return true;
		}

	}



	protected bool makePlayerBigger( )
    {
		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Make Player Bigger: No player has been set yet!");
			return false;
		}
        else
        {
			Debug.Log ("MakePlayerBigger()");
			iteractionObj.GetComponent<coreCharacterBehavior>().makeBigger();
			iteractionObj = null;
			return true;
		}
	}


    protected bool winGame()
    {
		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Win: No player has been set yet!");
			return false;
		}
        else
        {
			Instantiate(Resources.Load("WinCanvas"));
			iteractionObj.SetActive (false);
			iteractionObj = null;
            playerIsTouchingFlag = false;

			return true;
		}
	}



	protected bool loseGame()
    {

		if (iteractionObj == null)
        {
			Debug.Log ("Cannot Lose: No player has been set yet!");
			return false;
		}
        else
        {
			Instantiate(Resources.Load("LoseCanvas"));
			iteractionObj.SetActive (false);
			iteractionObj = null;
			playerIsTouchingFlag = false;
			return true;
		}
	}



	public void destroyObject()
    {
		Destroy (this.gameObject);
	}

	public void spinObject(bool shouldSpinTMP)
    {
		shouldSpin = shouldSpinTMP;

	}



	void OnCollisionExit(Collision collision)
    {

		if (collision.gameObject.GetComponent<coreCharacterBehavior> () != null)
        {
            playerIsTouchingFlag = false;
		}
	}

    // Change to a random color
    public void changeColor()
    {
        changeColor(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }

    public void changeColor(string color)
    {
        switch(color.ToLower())
        {
            case "red":
            case "r":
                changeColor(1.0f, 0, 0);
                break;
            case "blue":
            case "b":
                changeColor(0, 0, 1.0f);
                break;
            case "yellow":
            case "y":
                changeColor(1.0f, 1.0f, 0);
                break;
            case "green":
            case "g":
                changeColor(0, 1.0f, 0);
                break;
            case "orange":
            case "o":
                changeColor(1.0f, 0.5f, 0);
                break;
            case "purple":
            case "p":
                changeColor(0.6f, 0, 0.6f);
                break;
            case "pink":
                changeColor(1.0f, 0.25f, 1.0f);
                break;
            case "white":
            case "w":
                changeColor(1.0f, 1.0f, 1.0f);
                break;
            case "gray":
            case "grey":
                changeColor(0.6f, 0.6f, 0.6f);
                break;
            case "black":
            case "k":
                changeColor(0, 0, 0);
                break;
        }
    }

    public void changeColor(float r, float g, float b)
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("ChangeColorRPC", PhotonTargets.All, r, g, b); // *
        }
        else
        {
            // Make this character win
            ChangeColorRPC(r, g, b);
        }
    }

    [PunRPC]
    public void ChangeColorRPC(float r, float g, float b)
    {
        if (this.gameObject.GetComponent<Renderer>() == null)
        {
            Debug.LogWarning("RPC [ChangeColor] called on object with no Renderer");
            return;
        }

        this.gameObject.GetComponent<Renderer>().material.color = new Color(r, g, b);
    }
}
