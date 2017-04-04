using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
class MyHierarchyIcon
{
    static Texture2D goodTexture;
    static Texture2D badTexture;
    static List<int> goodObjects;
    static List<int> badObjects;

    static MyHierarchyIcon()
    {
        // Init
        goodTexture = AssetDatabase.LoadAssetAtPath("Assets/Editor/CrystalClearActionApply.png", typeof(Texture2D)) as Texture2D;
        badTexture = AssetDatabase.LoadAssetAtPath("Assets/Editor/White_X_in_red_background.png", typeof(Texture2D)) as Texture2D;
        EditorApplication.update += UpdateCB;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    static void UpdateCB()
    {
        // Check here
        GameObject[] go = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        goodObjects = new List<int>();
        badObjects = new List<int>();
        foreach (GameObject g in go)
        {
            // Example: mark all lights
            if (g.GetComponent<Light>() != null ||
                g.GetComponent<Camera>() != null ||
                g.GetComponent<UWBNetworkingPackage.NetworkManager>() != null ||
                g.GetComponent<SpawnScript>() != null
                )
                goodObjects.Add(g.GetInstanceID());
            else
                badObjects.Add(g.GetInstanceID());
        }

    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        if(goodObjects == null)
        {
            return;
        }

        // place the icoon to the right of the list:
        Rect r = new Rect(selectionRect);
        r.x = r.width - 20;
        r.width = 18;

        if (goodObjects.Contains(instanceID))
        {
            // Draw the texture if it's a light (e.g.)
            GUI.Label(r, goodTexture);
        }
        else if(badObjects.Contains(instanceID))
        {
            GUI.Label(r, badTexture);
        }
    }

}