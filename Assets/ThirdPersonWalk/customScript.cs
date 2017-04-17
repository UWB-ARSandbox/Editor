using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 newPosition = transform.position;
            newPosition.x += 10;

            PhotonView pV = transform.GetComponent<PhotonView>();
            pV.RPC("SetTargetPos", PhotonTargets.All, newPosition);
        }
    }
}
