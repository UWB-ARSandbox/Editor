using UnityEngine;

public class smaller : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		if (playerIsTouching())
		{
			makePlayerSmaller ();
		}
	}
	
}
