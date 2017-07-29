using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;

public class Collectable : coreObjectsBehavior {



	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		spinObject (true);
	}

	// updateGame() is called many times per
	// second
	void updateGame () {
		if(playerIsTouching()){
			givePoints (10);
			destroyObject();
		}

	}







}
