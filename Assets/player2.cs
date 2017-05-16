using UnityEngine;

public class player2 : coreCharacterBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		setGoal (10);
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		moveForward ();
		makeSmallerRPC ();
	}
	
}
