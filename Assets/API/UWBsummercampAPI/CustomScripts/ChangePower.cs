using UnityEngine;
using UWBsummercampAPI;

public class ChangePower : coreObjectsBehavior {

    public int powerID;
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
		
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		if (playerIsTouching())
        {
            takePoints(checkPoints());
            givePoints(powerID);
        }
	}
	
}
