﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makesmaller : coreObjectsBehavior {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (playerIsTouching()) {
			makePlayerSmaller();

		}
	}
}
