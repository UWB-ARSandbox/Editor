using UnityEngine;
using UWBsummercampAPI;


public class changeColor : coreObjectsBehavior {
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
        if (playerIsTouching())
        {
            changeColor("blue");

        }

    }
	
}
