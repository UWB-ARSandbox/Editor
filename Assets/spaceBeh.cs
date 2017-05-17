using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceBeh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.Instantiate("Cube", new Vector3(), new Quaternion(), 0);
        }
	}
}
