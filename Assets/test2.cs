using UnityEngine;
using UWBsummercampAPI;


public class test2 : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {

		if (playerIsTouching ()) {
			destroyObject ();
		}
		
	}
	
}
