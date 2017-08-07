using UnityEngine;
using UWBsummercampAPI;

public class changeColor1 : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		spinObject (true);
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		if (playerIsTouching ()) {
			changeColor ("red");

		}
	}
	
}
