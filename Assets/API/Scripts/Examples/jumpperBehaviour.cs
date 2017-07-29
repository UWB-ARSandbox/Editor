using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class jumpperBehaviour : coreObjectsBehavior
{

	// Use this for initialization
	void buildGame ()
    {
		Debug.Log ("AQUIIIII :  |"+this.GetType().FullName + "|END");

	}
	
	// Update is called once per frame
	void updateGame ()
    {

		if (playerIsTouching())
        {
			jumpPlayer();
		}

	}
}
