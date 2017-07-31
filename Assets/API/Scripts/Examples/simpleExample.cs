using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;




public class simpleExample : coreCharacterBehavior {

	public bool moving = false;
	// Use this for initialization
	void buildGame ()
    {
		//setGoal (2);

    }

    // Update is called once per frame
    void updateGame()
    {


		if (moving) {
			moveForward ();
		}
	
        

    }
}
