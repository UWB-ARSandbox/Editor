using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveClickToMove : MonoBehaviour
{

    public VRTK.VRTK_SimplePointer pointer;
    private bool lastPointerState = false;
	private GameObject GameManager;

	// Use this for initialization
	void Start ()
    {
        if (pointer == null)
            Debug.LogError("Missing pointer component on [ViveClickToMove]");

		GameManager = GameObject.Find("GameManager");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(lastPointerState && !pointer.pointerTip.gameObject.GetActive())
        {
            //PhotonNetwork.Instantiate("Cube", pointer.pointerTip.transform.position, new Quaternion(), 0);
			GameManager.GetComponent<UWBsummercampAPI.gameManagerBehavior>().getPlayer().GetComponent<UWBsummercampAPI.coreCharacterBehavior>().setClickToMove(pointer.pointerTip.transform.position);
        }

        lastPointerState = pointer.pointerTip.gameObject.GetActive();
    }
}
