using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class coreObjectsBehavior : MonoBehaviour
{
    // Flags
    protected bool playerIsTouchingFlag = false;
    protected bool shouldSpinFlag = false;
    protected bool timerRunningFlag = false;
    protected bool timerFinishedFlag = false;
    protected bool timerLoopFlag = false;
    protected bool hasTextFlag = false;

    // Dynamic Variables
    private GameObject iteractionObj = null;
    private GameObject lastSpawned = null;
    private TextMesh objText = null;
    private float timer = 0;
    private int timerMax = 10;

    // Constant Variables
    PhotonView pV;

    #region Unity Methods
    // Use this for initialization
    void Start()
    {
        pV = transform.GetComponent<PhotonView>();

        MethodInfo method = this.GetType().GetMethod("buildGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }

        if (pV != null && PhotonNetwork.connectionStateDetailed == ClientState.Joined)
        {
            pV.RPC("RequestColorRPC", PhotonTargets.MasterClient);
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

        if (shouldSpinFlag)
        {
            transform.Rotate(0, 20 * Time.deltaTime, 0);
        }

        // Consume timer event
        timerFinishedFlag = false;

        // Determine if timer is finished
        if (timerRunningFlag)
        {
            timer = timer + Time.deltaTime;

            if(timer > timerMax)
            {
                timerFinishedFlag = true;
                if(timerLoopFlag)
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

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with [" + collision.gameObject.name + "]");
        if (collision.gameObject.GetComponent<coreCharacterBehavior>() != null)
        {
            iteractionObj = collision.gameObject;
            playerIsTouchingFlag = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<coreCharacterBehavior>() != null)
        {
            playerIsTouchingFlag = false;
            /* If we set interactionObj to false we are unable to do things like
             * set the player invisible after a second
            if (collision.gameObject == iteractionObj)
            {
                iteractionObj = null;
            }
            */
        }
    }
    #endregion

    protected bool playerIsTouching()
    {
        // Consume touching event? We consume
        //  it in every other event
        bool retVal = playerIsTouchingFlag;
        playerIsTouchingFlag = false;
        return retVal;
    }

    #region Make-Player-Do Methods 
    protected bool teleportPlayerForward(int distance = 5)
    {
        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Teleport: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("TeleportForward");

            iteractionObj.GetComponent<coreCharacterBehavior>().teleportForward(distance);
            return true;
        }
    }

    protected bool turnPlayerLeft()
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Turn Left: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Left");
            iteractionObj.GetComponent<coreCharacterBehavior>().turnLeft();
            return true;
        }

    }
    
    protected bool turnPlayerRight()
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Turn Right: No player has been set yet!");
            return false;
        }
        else
        {
            iteractionObj.GetComponent<coreCharacterBehavior>().turnRight();
            return true;
        }

    }

    protected bool turnPlayerAround()
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Turn Right: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Around");
            iteractionObj.GetComponent<coreCharacterBehavior>().turnAround();
            return true;
        }

    }

    protected bool jumpPlayer(int forceTMP = 10)
    {
        int force = forceTMP * 50;

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Jump: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("jump");
            iteractionObj.GetComponent<coreCharacterBehavior>().jump();
            
            return true;
        }

    }

    protected bool makePlayerSmaller()
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Make Player Smaller: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("MakePlayerSmaller()");
            iteractionObj.GetComponent<coreCharacterBehavior>().makeSmaller();
            
            return true;
        }

    }

    protected bool makePlayerBigger()
    {
        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Make Player Bigger: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("MakePlayerBigger()");
            iteractionObj.GetComponent<coreCharacterBehavior>().makeBigger();
            
            return true;
        }
    }

    public bool makePlayerInvisible()
    {
        if (iteractionObj == null)
        {
            Debug.Log("Cannot make Player Invisble: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Make Player Invisible");
            iteractionObj.GetComponent<coreCharacterBehavior>().makeInvisible();

            return true;
        }
    }

    public bool makePlayerVisible()
    {
        if (iteractionObj == null)
        {
            Debug.Log("Cannot make Player Visible: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Make Player Visible");
            iteractionObj.GetComponent<coreCharacterBehavior>().makeVisible();

            return true;
        }
    }

    public bool setPlayerText(string text)
    {
        if (iteractionObj == null)
        {
            Debug.Log("Cannot set Player Text: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Set Player Text");
            iteractionObj.GetComponent<coreCharacterBehavior>().setText(text);

            return true;
        }
    }
    #endregion

    public void spinObject(bool shouldSpinTMP)
    {
        shouldSpinFlag = shouldSpinTMP;

    }

    public void teleportObjectForward(int distance = 5)
    {
        Vector3 newPosition = gameObject.transform.position;
        newPosition += gameObject.transform.forward * distance;
        gameObject.transform.position = newPosition;
    }

    #region Visibility Methods
    public void makeObjectInvisible()
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeObjInvsRPC", PhotonTargets.All); // *
        }
        else
        {
            // Make this object invisible
            makeObjInvsRPC();
        }
    }

    [PunRPC]
    public void makeObjInvsRPC()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void makeObjectVisible()
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeObjVisRPC", PhotonTargets.All); // *
        }
        else
        {
            // Make this object visible
            makeObjVisRPC();
        }
    }

    [PunRPC]
    public void makeObjVisRPC()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public bool isObjectVisible()
    {
        return gameObject.GetComponent<Renderer>().isVisible;
    }
    #endregion

    #region Change Color Methods
    // Change to a random color
    public void changeColor()
    {
        changeColor(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }

    public void changeColor(string color)
    {
        switch (color.ToLower())
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

    [PunRPC]
    public void RequestColorRPC()
    {
        Debug.Log("Request Color RPC");
        PhotonView phov = gameObject.GetComponent<PhotonView>();
        Color thisColor = gameObject.GetComponent<Renderer>().material.color;
        phov.RPC("ChangeColorRPC", PhotonTargets.All, thisColor.r, thisColor.g, thisColor.b);
    }
    #endregion

    #region Game Manager Methods
    protected bool givePoints(int points = 10)
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot give points: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Points Given");
            iteractionObj.GetComponent<coreCharacterBehavior>().addPoints(points);
            return true;
        }

    }

    protected bool takePoints(int points = 10)
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot take points: No player has been set yet!");
            return false;
        }
        else
        {
            Debug.Log("Points Taken");
            iteractionObj.GetComponent<coreCharacterBehavior>().addPoints(-points);
            return true;
        }

    }

    protected bool winGame()
    {
        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Win: No player has been set yet!");
            return false;
        }
        else
        {
            iteractionObj.GetComponent<coreCharacterBehavior>().winGame();
            playerIsTouchingFlag = false;
            return true;
        }
    }

    protected bool loseGame()
    {

        if (iteractionObj == null)
        {
            Debug.LogWarning("Cannot Lose: No player has been set yet!");
            return false;
        }
        else
        {
            iteractionObj.GetComponent<coreCharacterBehavior>().loseGame();
            playerIsTouchingFlag = false;
            return true;
        }
    }

    public void setLevelTimer(int timerLength, bool makeRepeat = false)
    {
        gameManagerBehavior.instance.setTimer(timerLength, makeRepeat);
    }

    public void makeLevelTimerRepeat(bool makeRepeat)
    {
        gameManagerBehavior.instance.makeTimerRepeat(makeRepeat);
    }

    public void startLevelTimer()
    {
        gameManagerBehavior.instance.startTimer();
    }

    public void stopLevelTimer()
    {
        gameManagerBehavior.instance.stopTimer();
    }

    public bool levelTimeIsUp()
    {
        return gameManagerBehavior.instance.timeIsUp();
    }

    public bool levelTimerIsRunning()
    {
        return gameManagerBehavior.instance.timerIsRunning(); ;
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

    #region Creation / Deletion

    // Note: This method only attatches scripts that are of coreObjectBehavior, NOT scripts that are of
    //  corePlayerBehavior. The assumption is made that player creation is too complex of a task for students
    //  to do in script.
    public void createNewObject(string objectName)
    {
        // Check to see if they're continuously spawning the same object
        //  This lets us avoid many time-consuming 'Find' calls
        if (lastSpawned == null || lastSpawned.name != objectName)
        {
            lastSpawned = GameObject.Find(objectName);
        }

        if (lastSpawned != null)
        {
            GameObject newObj;
            if (lastSpawned.GetComponent<PhotonView>() != null)
            {
                // Recreate object
                newObj = PhotonNetwork.Instantiate(lastSpawned.GetComponent<SpawnScript>().prefabName, 
                                                    gameObject.transform.position, gameObject.transform.rotation, 0);
                newObj.transform.localScale = lastSpawned.transform.localScale;

                // Attatch relevant scripts
                MonoBehaviour[] scriptsOnObject = lastSpawned.GetComponents<coreObjectsBehavior>();
                foreach (MonoBehaviour script in scriptsOnObject)
                {
                    newObj.AddComponent(script.GetType());
                }

                // Fix color
                Color nColor =  lastSpawned.GetComponent<Renderer>().material.color;
                coreObjectsBehavior nBehavior = newObj.GetComponent<coreObjectsBehavior>();
                if (nBehavior != null)
                {
                    nBehavior.changeColor(nColor.r, nColor.g, nColor.b);
                }
            }
            else
            {
                Instantiate(lastSpawned, gameObject.transform.position, gameObject.transform.rotation);
            }
        }

    }

    public void destroyObject()
    {
        Destroy(this.gameObject);
    }
    #endregion

    #region Display Text
    public void setText(string text)
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("setTextRPC", PhotonTargets.All, text); // *
        }
        else
        {
            // Set this object's text
            setTextRPC(text);
        }
    }

    [PunRPC]
    public void setTextRPC(string text)
    {
        if (objText == null)
            createText();

        objText.text = text;
    }

    private void createText()
    {
        GameObject objTextGO = new GameObject();
        objTextGO.transform.parent = gameObject.transform;
        objTextGO.transform.localPosition = new Vector3(0, gameObject.transform.localScale.y, 0);
        objText = objTextGO.AddComponent<TextMesh>();
        objText.anchor = TextAnchor.LowerCenter;
    }
    #endregion
}
