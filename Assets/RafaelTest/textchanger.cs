using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textchanger : MonoBehaviour {
	private TextMesh textstring;

	void Start () {
		textstring = transform.GetComponentInChildren<TextMesh> ();


		changeText ("YEAH!!!");
		Debug.Log ("test");
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    [PunRPC]
	public void changeText(string changeTMP)
    {
		textstring.text = changeTMP;

	}
}
