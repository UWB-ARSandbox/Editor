using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coreCollectableBehaviour : coreObjectsBehavior {

	private bool shouldSpin = false;


	void Update(){

		if (shouldSpin){

			transform.Rotate(0,20*Time.deltaTime,0);

		}

	}

	public void destroyObject(){
		Destroy (this.gameObject);
	}

	public void spinObject(bool shouldSpinTMP){


	}


}
