using UnityEngine;

public class destroy : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	int health = 2;
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
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
