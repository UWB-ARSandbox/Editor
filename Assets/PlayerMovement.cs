using UnityEngine;

public class PlayerMovement : coreCharacterBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {

		setGoal (10);
		moveForward ();	
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		
	}
	
}
