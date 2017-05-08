using UnityEngine;

public class WinBlock : coreObjectsBehavior
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

		if (playerIsTouching())
        {
			winGame();
		}

	}
}
