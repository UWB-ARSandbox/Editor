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
    protected bool startupFlag = true;
    protected bool clickMoveFlag = false;
    protected bool isMovingFlag = false;
    protected bool isJumpingFlag = false;
    protected bool timerRunningFlag = false;
    protected bool timerFinishedFlag = false;
    protected bool timerLoopFlag = false;

    // Dynamic Variables
    private Vector3 destPoint;
    private float clickMovementSpeed = 10.0f;
    private float yAxis;
    protected sizes characterSize = sizes.normal;
    private GameObject lastSpawned = null;
    private TextMesh objText = null;
    protected int speed;
    private float timer = 0;
    private int timerMax = 10;

    // Constant Variables
    PhotonView pV;
    MethodInfo startMethod;
    MethodInfo updateMethod;

    #region Unity Methods
    // Use this for initialization
    void Start()
    {
        pV = transform.GetComponent<PhotonView>();

        startMethod = this.GetType().GetMethod("buildGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        updateMethod = this.GetType().GetMethod("updateGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

        yAxis = gameObject.transform.position.y - gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for the client to connect to the server before updating
        if (startupFlag && !isConnected())
        {
            // If we're not networked, skip this step
            if (pV == null)
                startupFlag = false;
            return;
        }
        else if(startupFlag)
        {
            startupFlag = false;
            if (startMethod != null)
            {
                startMethod.Invoke(this, new object[0]);
            }
        }
        
        if (updateMethod != null)
        {
            updateMethod.Invoke(this, new object[0]);
        }

        if(clickMoveFlag)
        {
            //check if the screen is touched / clicked   
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButton(0)))
            {
                //declare a variable of RaycastHit struct
                RaycastHit hit;

                //Create a Ray on the tapped / clicked position
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //for unity editor
                #if UNITY_EDITOR
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //for touch device
                #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                #endif

                //Check if the ray hits any collider
                if (Physics.Raycast(ray, out hit))
                {
                    //set a flag to indicate to move the gameobject
                    isMovingFlag = true;
                    //save the click / tap position
                    destPoint = hit.point;
                    //as we do not want to change the y axis value based on touch position, reset it to original y axis value
                    destPoint.y = yAxis;
                    Debug.Log(destPoint);
                }

            }

            //check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
            //if (flag && !Mathf.Approximately(gameObject.transform.position.magnitude/512, endPoint.magnitude/512))
            if (isMovingFlag && Vector2.Distance(gameObject.transform.position, destPoint) > 1)
            { //&& !(V3Equal(transform.position, endPoint))){
              //move the gameobject to the desired position
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destPoint, 1 / (clickMovementSpeed * (Vector3.Distance(gameObject.transform.position, destPoint))));
            }

            //set the movement indicator flag to false if the endPoint and current gameobject position are equal
            // else if (flag && Mathf.Approximately(gameObject.transform.position.magnitude / 512, endPoint.magnitude / 512))
            else if (isMovingFlag)
            {
                isMovingFlag = false;
                Debug.Log("I am here");
            }
        }
        else if (isMovingFlag)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
    #endregion

    #region Public Network Methods
    public bool isConnected()
    {
        return PhotonNetwork.connectionStateDetailed == ClientState.Joined;
    }
    #endregion

    #region Movement
    public void moveForward(int speedTMP = 5)
    {
        speed = speedTMP;
        isMovingFlag = true;
    }

    public void stopMoving()
    {

        isMovingFlag = false;
    }

    public void setClickToMove(bool clickToMove)
    {
        clickMoveFlag = clickToMove;
    }

    [PunRPC]
    public void setDestinationRPC(Vector3 position)
    {
        if (clickMoveFlag)
        {
            isMovingFlag = true;
            //save the click / tap position
            destPoint = position;
            //as we do not want to change the y axis value based on touch position, reset it to original y axis value
            destPoint.y = yAxis;
        }
    }

    public bool isMoving()
    {
        return isMovingFlag;
    }
    
    public void teleportForward(int distance = 5)
    {
        Vector3 newPosition = gameObject.transform.position;
        newPosition += gameObject.transform.forward*distance;
        gameObject.transform.position = newPosition;
    }

    public void turnLeft()
    {
        // Shave off excess rotation to (TRY to) ensure moving in a cardinal direction
        double newY = Math.Round(gameObject.transform.rotation.eulerAngles.y / 90);
        newY = ((int)newY - 1) % 4;

        gameObject.transform.rotation = Quaternion.Euler(0, (int)newY * 90, 0);
    }


    public void turnRight()
    {
        // Shave off excess rotation to (TRY to) ensure moving in a cardinal direction
        double newY = Math.Round(gameObject.transform.rotation.eulerAngles.y / 90);
        newY = ((int)newY + 1) % 4;

        gameObject.transform.rotation = Quaternion.Euler(0, (int)newY * 90, 0);
    }

    public void turnAround()
    {
        // Shave off excess rotation to (TRY to) ensure moving in a cardinal direction
        double newY = Math.Round(gameObject.transform.rotation.eulerAngles.y / 90);
        newY = ((int)newY + 2) % 4;

        gameObject.transform.rotation = Quaternion.Euler(0, (int)newY * 90, 0);
    }
    #endregion

    #region Bigger/Smaller
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
    #endregion


    public void jump(int forceTMP = 10)
    {
        int force = forceTMP * 50;
        GetComponent<Rigidbody>().AddForce(0, force, 0);
    }
    
    public bool isJumping()
    {
        return isJumpingFlag;
    }

    #region Visibility
    public void makeInvisible()
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeInvsRPC", PhotonTargets.All); // *
        }
        else
        {
            // Make player invisible
            makeInvsRPC();
        }
    }

    [PunRPC]
    public void makeInvsRPC()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void makeVisible()
    {
        if (pV != null)
        {
            // This file PunRPC
            pV.RPC("makeVisRPC", PhotonTargets.All); // *
        }
        else
        {
            // Make player visible
            makeVisRPC();
        }
    }

    [PunRPC]
    public void makeVisRPC()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public bool isVisible()
    {
        return gameObject.GetComponent<Renderer>().isVisible;
    }
    #endregion

    #region GameManager Methods
    public void winGame()
    {
        stopMoving();
        gameManagerBehavior.instance.winGame();
    }

    public void loseGame()
    {
        stopMoving();
        gameManagerBehavior.instance.loseGame();
    }

    public void addPoints(int pointsTMP = 10)
    {
        gameManagerBehavior.instance.addPoints(pointsTMP);
    }

    public void removePoints(int pointsTMP = 10)
    {
        gameManagerBehavior.instance.addPoints(-pointsTMP);
    }

    protected void setGoal(int goalPointsTMP)
    {
        gameManagerBehavior.instance.setGoal(goalPointsTMP);
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
                Color nColor = lastSpawned.GetComponent<Renderer>().material.color;
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
