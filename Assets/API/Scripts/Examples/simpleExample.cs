using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;




public class simpleExample : coreCharacterBehavior {


	// Use this for initialization
	void buildGame ()
    {
		setGoal (10);

    }

    // Update is called once per frame
    void updateGame()
    {
		moveForward();

	
        

    }
}
