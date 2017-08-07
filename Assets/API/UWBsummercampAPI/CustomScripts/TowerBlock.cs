using UnityEngine;
using UWBsummercampAPI;

public class TowerBlock : coreObjectsBehavior {

    public const int MAKE_BIGGER_POINTS = 1;
    public const int MAKE_SMALLER_POINTS = 2;
    public const int DISAPPEAR_POINTS = 3;
    public const int CHANGE_COLOR_POINTS = 4;

    private const float SIZE_FACTOR = 3f;
    private const int TELEPORT_DISTANCE = 3000;

    private Vector3 originalScale;
	
	// buildGame() is called once, at the start
	// of the game
	void buildGame () {
        originalScale = transform.localScale;
	}
	
	// updateGame() is called many times per
	// second
	void updateGame () {
		if (playerIsTouching())
        {
            switch (checkPoints())
            {
                case MAKE_BIGGER_POINTS:
                    transform.Translate(new Vector3(0f, transform.localScale.y / -2f));
                    transform.localScale = originalScale * SIZE_FACTOR;
                    transform.Translate(new Vector3(0f, transform.localScale.y / 2f));
                    break;
                case MAKE_SMALLER_POINTS:
                    transform.localScale = originalScale / SIZE_FACTOR;
                    break;
                case DISAPPEAR_POINTS:
                    destroyObject();
                    break;
                case CHANGE_COLOR_POINTS:
                    changeColor();
                    break;
                default:
                    break;
            }
            takePoints(checkPoints());
            turnPlayerAround();
            teleportPlayerForward(TELEPORT_DISTANCE);
        }
	}
	
}
