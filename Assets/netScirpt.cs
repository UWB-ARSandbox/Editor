using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class netScirpt : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonView photonView = GameObject.Find("RafaelCube(Clone)").GetComponent<PhotonView>();
            photonView.RPC("changeText", PhotonTargets.All, "New text");
        }

    }
}
