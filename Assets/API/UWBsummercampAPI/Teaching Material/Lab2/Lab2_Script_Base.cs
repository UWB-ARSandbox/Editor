using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab2_Script_Base : coreCharacterBehavior {

	//buildGame is for things you want to set before the game runs
	void buildGame () {
		//Insert Code Here
		setGoal (10);
		moveForward ();


	}
	
	// updateGame is for things you want to run alongside the game
	void updateGame () {
		//Insert Code Here

	}
}
