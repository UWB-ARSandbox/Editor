using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getForward : coreCharacterBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		moveForward ();
	}
	
}
