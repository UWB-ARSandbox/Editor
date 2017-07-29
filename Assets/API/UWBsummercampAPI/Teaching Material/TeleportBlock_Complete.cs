using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;


public class TeleportBlock_Complete : coreObjectsBehavior {

	//buildGame is for things you want to set before the game runs
	void buildGame () {
		//Insert Code Here

	}

	// updateGame is for things you want to run alongside the game
	void updateGame () {
		//Insert Code Here
		if (playerIsTouching ()) {
			teleportPlayerForward ();

		}
	}
}
