using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UWBsummercampAPI {


public class CanvasManager : MonoBehaviour {
	public  GameObject dashboard;
	public 	Text	dashboardText;

	public  GameObject gameOver;
	public  Text gameOverText;


	// Use this for initialization
	void Start () {


		gameOver.SetActive (false);
		dashboard.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}



		public void refreshDashboard(string dashMsg){
			dashboard.gameObject.SetActive (true);


			dashboardText.text = dashMsg;

		}


}

}