using UnityEngine;

public class turnRight : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		if (playerIsTouching())
		{
			turnPlayerRight ();
		}
	}
	
}
