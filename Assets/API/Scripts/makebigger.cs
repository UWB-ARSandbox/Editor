﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makebigger : coreObjectsBehavior {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (playerIsTouching()) {
			makePlayerBigger();

		}
	}
}