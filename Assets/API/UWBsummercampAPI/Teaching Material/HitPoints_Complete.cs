using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;

public class HitPoints_Complete : coreObjectsBehavior {
	//Insert Variable Here
	int health = 2;

	//buildGame is for things you want to set before the game runs
	void buildGame () {
		//Insert Code Here

	}

	// updateGame is for things you want to run alongside the game
	void updateGame () {
		//Insert Code Here
		if (playerIsTouching ()) {
			if (health == 0) {
				destroyObject ();
			} 
			else {
				health -= 1;
				turnPlayerAround ();
			}
		}
	}
}
