using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : coreCharacterBehavior {


	// Use this for initialization
	void buildGame ()
    {
		
	}

    // Update is called once per frame
    void updateGame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            crouch();
        }
        //moveForward ();
    }
}
