﻿//C# Example
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CreationWindow : EditorWindow
{
    static bool physicalObject = false;

	static List<string> names = new List<string>() { "Cube", "Sphere", "Capsule", "Cylinder", "Plane", "Quad",
		"PhysicCube", "PhysicSphere", "PhysicCapsule", "PhysicCylinder", "PhysicPlane", "PhysicQuad", "Head",
		"HandLeft", "HandRight", "LeftHand", "RightHand", "HoloHead", "ViveHead", "VirtualCamera"};

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/[UWB] Creation Menu", false, 3)]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CreationWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Object Settings", EditorStyles.boldLabel);
        physicalObject = EditorGUILayout.Toggle("Enable Physics", physicalObject);

        /* Each Primitive Object requires it's own special method */

        GUILayout.Label("Primitive Objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Cube"))
        {
            CreateCube();
        }
        else if(GUILayout.Button("Sphere"))
        {
            CreateSphere();
        }
        else if(GUILayout.Button("Capsule"))
        {
            CreateCapsule();
        }
        else if(GUILayout.Button("Cylinder"))
        {
            CreateCylinder();
        }
        else if(GUILayout.Button("Plane"))
        {
            CreatePlane();
        }
        else if (GUILayout.Button("Quad"))
        {
            CreateQuad();
        }

        /* Load Custom Objects */

        GUILayout.Label("Custom Objects", EditorStyles.boldLabel);

        string[] searchFolders = new string[1];
        searchFolders[0] = "Assets/Resources";
        string[] guids = AssetDatabase.FindAssets("t:Prefab",searchFolders);

        for(int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            string objName = path.Split('/').Last().Split('.').First(); // 17 for Assets/Resources/ and 7 for .prefab
            //string objName = path.Substring(17, path.Length - 24);
			for(int j = 0; j < names.Count; j++)
			{
				if(objName == names[j])
				{
					goto skipButton;
				}
			}
            if(GUILayout.Button(objName))
            {
                CreateObj(objName, path);
            }
			skipButton: ;
        }
    }

    public static void CreateObj(string objName, string path)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(asset) as GameObject;
        InitShape(obj, objName);
    }

    #region Create Primitives
    [MenuItem("GameObject/3D Object/Cube", false, 0)]
    public static void CreateCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        InitShape(cube, "Cube");
    }

    [MenuItem("GameObject/3D Object/Sphere", false, 1)]
    public static void CreateSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        InitShape(sphere, "Sphere");
    }

    [MenuItem("GameObject/3D Object/Capsule", false, 2)]
    public static void CreateCapsule()
    {
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        InitShape(capsule, "Capsule");
    }

    [MenuItem("GameObject/3D Object/Cylinder", false, 3)]
    public static void CreateCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        InitShape(cylinder, "Cylinder");
    }

    [MenuItem("GameObject/3D Object/Plane", false, 4)]
    public static void CreatePlane()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        InitShape(plane, "Plane");
    }

    [MenuItem("GameObject/3D Object/Quad", false, 5)]
    public static void CreateQuad()
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        InitShape(quad, "Quad");
        // Quads have a different default spawn behavior
        quad.transform.LookAt(GetViewCenterWorldPos(0));
        quad.transform.Rotate(0, 180, 0);
    }
    #endregion

    // This method sets an object's shape to center, and attatches the proper script to it
    public static void InitShape(GameObject obj, string prefabName)
    {
        if (Selection.activeTransform != null)
        {
            obj.transform.parent = Selection.activeTransform;
            obj.transform.position = Selection.activeTransform.position;
        }
        else
        {
            obj.transform.position = GetViewCenterWorldPos();
        }

        SpawnScript spawnScript = obj.GetComponent<SpawnScript>();
        if(spawnScript == null)
            spawnScript = obj.AddComponent<SpawnScript>();
        spawnScript.prefabName = prefabName; // Set our script's name

        UWBPhotonTransformView objTview = obj.GetComponent<UWBPhotonTransformView>();
        if(objTview == null)
            objTview = obj.AddComponent<UWBPhotonTransformView>(); // Attatch this and a PhotonView
        objTview.enableSyncPos(); // These are all off by default
        objTview.enableSyncRot(); // 
        objTview.enableSyncScale(); // 

        PhotonView objPview = obj.GetComponent<PhotonView>();
        objPview.ObservedComponents = new List<Component>(); // Make sure we're observing this object
        objPview.ObservedComponents.Add(obj.GetComponent<Transform>());
        objPview.synchronization = ViewSynchronization.UnreliableOnChange; // Default
        objPview.ownershipTransfer = OwnershipOption.Takeover;
        if (physicalObject)
        {
            if(obj.GetComponent<PhotonRigidbodyView>() == null)
                obj.AddComponent<PhotonRigidbodyView>(); // Track velocities; rigidbody automatically added
            spawnScript.prefabName = "Physic" + prefabName; // Ensure the proper prefab name for client download
            obj.name = "Physic" + obj.name; // For convience, also change 
        }
    }

    [MenuItem("GameObject/2D Object", false, 51)]
    [MenuItem("GameObject/2D Object/Sprite", true)]
    [MenuItem("GameObject/3D Object/Ragdoll...", false, 0)]
    public static void EmptyMethod()
    {
        // Do nothing
        // Overrides basic MenuItem functionality (such as 'Create Cube')
    }

    [MenuItem("GameObject/2D Object", true)]
    [MenuItem("GameObject/2D Object/Sprite", true)]
    [MenuItem("GameObject/3D Object/Ragdoll...", true)]
    static bool ValidateMenuItem()
    {
        return false;
    }

    // Get a position 5f in front of the center of the screen, and return that position
    private static Vector3 GetViewCenterWorldPos()
    {
        Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        Vector3 worldPos = worldRay.GetPoint(5f);

        return worldPos;
    }

    // Get a position at the center of the screen, and return that position
    private static Vector3 GetViewCenterWorldPos(float distance)
    {
        Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        Vector3 worldPos = worldRay.GetPoint(distance);

        return worldPos;
    }
}