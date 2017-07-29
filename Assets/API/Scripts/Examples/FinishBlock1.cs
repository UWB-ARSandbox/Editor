using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWBsummercampAPI;

public class FinishBlock : coreObjectsBehavior {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (playerIsTouching()) {
			loseGame();
		}

	}
}
