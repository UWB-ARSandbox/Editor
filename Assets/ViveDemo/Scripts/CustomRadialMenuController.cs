using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Events;
using Photon;
using VRTK;

public class CustomRadialMenuController : UnityEngine.MonoBehaviour
{
    Transform controllerTip;
    RadialMenu radialMenu;

    public Object[] buttons;

    // Use this for initialization
    void Awake()
    {
        if (radialMenu == null)
        {
            radialMenu = transform.parent.Find("RadialMenu/RadialMenuUI/Panel").GetComponent<RadialMenu>();
        }

        // Look for controller tip on awake
        if (controllerTip == null)
        {
            controllerTip = transform.parent.transform.Find("Model/tip/attach");
        }
    }

    void Start()
    {
        if (radialMenu == null)
        {
            radialMenu = transform.parent.Find("RadialMenu/RadialMenuUI/Panel").GetComponent<RadialMenu>();

            if (radialMenu == null)
            {
                Debug.LogError("Could not find radial menu in parent");
            }
        }

        // Look for controller tip on start
        if (controllerTip == null)
        {
            controllerTip = transform.parent.transform.Find("Model/tip/attach");
        }

        if (radialMenu != null)
        {
            for(int i = 0; i < buttons.Length; i++)
            {
                Texture2D newTexture = AssetPreview.GetAssetPreview(buttons[i]);
                Debug.Log("Create button preview for " + buttons[i].name);
                if (newTexture == null)
                {
                    Debug.LogError("Null Texture created for RadialMenu button");
                }
                else
                {
                    Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
                    radialMenu.buttons[i].ButtonIcon = newSprite;
                    Object objClone = buttons[i];
                    radialMenu.buttons[i].OnClick.AddListener(delegate { spawnObj(objClone); });
                }
            }
        }
    }

	void Update()
    {
        // Continue to look for controller tip
		if (controllerTip == null)
		{
			controllerTip = transform.parent.transform.Find("Model/tip/attach");

			if (controllerTip != null)
			{
				Debug.Log("Controller tip found in parent");
			}
		}
	}

    public void spawnObj(Object obj)
    {
        //GameObject g = GameObject.Instantiate((GameObject)obj);
        PhotonNetwork.Instantiate(obj.name, controllerTip.position, controllerTip.rotation, 0);
    }
}
