﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UWBsummercampAPI;


public class jumpperBehaviour : coreObjectsBehavior
{

	// Use this for initialization
	void buildGame ()
    {


	}
	
	// Update is called once per frame
	void updateGame ()
    {

		if (playerIsTouching())
        {
			jumpPlayer();
		}

	}
}
