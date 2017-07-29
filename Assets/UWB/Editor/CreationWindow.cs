using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UWBNetworkingPackage;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


[InitializeOnLoad]
public class CreationWindow : EditorWindow
{
    // Active Object
    static GameObject selectedObject;
    static MonoBehaviour selectedScript;
    static string scriptTypeLabel;

    // Active Scene
    static Scene selectedScene;
    static int selectedSceneIdx;
    static int newSceneIdx;
	static string scenePath = "API/UWBsummercampAPI/Scenes/";
    static string[] sceneNames;// = new string[] { "1DemoScene", "2DemoScene", "3DemoSceneSmallBig" };

	//Scripts Handle
	//static Scene selectedScene;
	static int selectedScriptIdx ;
	static int newScriptIdx ;
	static string customScriptsPath = "API/UWBsummercampAPI/CustomScripts/";
	static string[] customScriptNames;



    // Network Variables
    static NetworkManager netManager;
    static string roomName;
    static bool isServer;

    // Folding sections
    static bool objectFold = true;
    static bool scriptFold = true;
    static bool levelFold = false;
    static bool networkFold = true;
    static bool createSettingsFold = true;
    static bool primitiveObjFold = true;
    static bool customObjFold = true;
	static bool customScriptsFold = true;


    // GUI Specific variables
    static string networkStatus;
    static GUIStyle networkStatusStyle;
    static Vector2 scrollPane;

    // Specific Creation Settings
    static bool physicalObject = false;
    static Material defaultMaterial = null;

    // Prefabs to ignore in Custom Objects button population
	static List<string> ignoreNames = new List<string>() { "Cube", "Sphere", "Capsule", "Cylinder", "Plane", "Quad",
		"PhysicCube", "PhysicSphere", "PhysicCapsule", "PhysicCylinder", "PhysicPlane", "PhysicQuad", "Head",
		"HandLeft", "HandRight", "LeftHand", "RightHand", "HoloHead", "ViveHead", "VirtualCamera"};

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/[UWB] Creation Menu", false, 3)]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CreationWindow));
    }

    // Called once, when the window is first opened
    void Awake()
    {
        networkStatusStyle = new GUIStyle();
        networkStatusStyle.fontStyle = FontStyle.Bold;
        networkStatusStyle.normal.textColor = Color.black;

        RefreshNetworkManager();

        selectedScene = EditorSceneManager.GetActiveScene();

        RefreshSceneNames();
		RefreshScriptNames ();
    }

    // This happens more than once, including when the play button is pressed but 
    //  just before the game is actually playing via Application.isPlaying()
    private void OnEnable()
    {
        Awake();
    }

    void RefreshNetworkManager()
    {
        // If we've changed our network manager's settings
        if (netManager != null && (!netManager.RoomName.Equals(roomName) || netManager.MasterClient != isServer))
        {
            netManager.RoomName = roomName;
            netManager.MasterClient = isServer;
            EditorUtility.SetDirty(netManager); // Fairly expensive operation
        }
        else // Find our network manager component in the scene
        {
            GameObject netManObj = GameObject.Find("NetworkManager");
            if (netManObj != null)
            {
                netManager = netManObj.GetComponent<NetworkManager>();
                roomName = netManager.RoomName;
                isServer = netManager.MasterClient;
            }
            else
            {
                roomName = "<N/A>";
                isServer = false;
            }
        }
        RefreshNetworkConnection();
    }

    void RefreshNetworkConnection()
    {
        // Determine our connection status 
        // (Ready, Unable, Loading, Connecting, Connected, Disconnected)
        networkStatus = "...";

        if(netManager == null)
        {
            networkStatus = "Network Manager missing..";
            networkStatusStyle.normal.textColor = Color.red;
            return;
        }

        if (Application.isPlaying)
        {
            switch(PhotonNetwork.connectionState)
            {
                case ConnectionState.InitializingApplication:
                    networkStatus = "Loading..";
                    networkStatusStyle.normal.textColor = Color.blue;
                    break;
                case ConnectionState.Connecting:
                    networkStatus = "Connecting..";
                    networkStatusStyle.normal.textColor = Color.blue;
                    break;
                case ConnectionState.Connected:
                    if (PhotonNetwork.connectionStateDetailed == ClientState.Joined)
                    {
                        networkStatus = "Connected";
                        networkStatusStyle.normal.textColor = Color.green;
                    }
                    else
                    {
                        networkStatus = "Connecting..";
                        networkStatusStyle.normal.textColor = Color.blue;
                    }
                    break;
                case ConnectionState.Disconnecting:
                    networkStatus = "Disconnecting..";
                    networkStatusStyle.normal.textColor = Color.red;
                    break;
                case ConnectionState.Disconnected:
                    networkStatus = "Disconnected..";
                    networkStatusStyle.normal.textColor = Color.red;
                    break;
            }
        }
        else
        {
            networkStatus = "Ready to Connect!";
            networkStatusStyle.normal.textColor = Color.green;
        }
    }

    void ChangeScene()
    {
        // newScene is a global static variable to prevent
        //  reinstantiation every frame
        selectedSceneIdx = newSceneIdx;

        // If changes the the scene have been made, save them
        if(selectedScene.isDirty)
            EditorSceneManager.SaveScene(selectedScene);

        selectedScene = EditorSceneManager.OpenScene(Application.dataPath + "/" + scenePath + sceneNames[selectedSceneIdx] + ".unity");
    }

    void RefreshSceneNames()
    {
        string[] searchFolders = new string[1];
        searchFolders[0] = "Assets/" + scenePath.TrimEnd('/');
        string[] guids = AssetDatabase.FindAssets("t:Scene", searchFolders);

        sceneNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            string sceneName = path.Split('/').Last().Split('.').First();
            sceneNames[i] = sceneName;
        }
    }

	void RefreshScriptNames()
	{
		string searchFolders = "Assets/" + customScriptsPath.TrimEnd('/');
	//	string[] guids = AssetDatabase.FindAssets("t:Scene", searchFolders);



		DirectoryInfo dir = new DirectoryInfo(searchFolders);
		FileInfo[] info = dir.GetFiles("*.cs");

		customScriptNames = new string[info.Length+1];
		customScriptNames [0] = "No Script Selected";

		for (int i = 0; i < info.Length; i++)
		{
			customScriptNames[i+1] = info[i].Name.Trim('.','c','s');
		
		}




	}



    // This ensures the menu will update whenever an object
    //  would normally update. This means that object names
    //  and scripts update properly.
    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    // This describes the look of the window, as well as the
    //  functionality of all of the components, such as what
    //  a particular button does.
    void OnGUI()
    {
        // Allows the entire menu to be scrollable if too small
        scrollPane = GUILayout.BeginScrollView(scrollPane);

        #region Selected Object stats
        if (Selection.activeGameObject)
        {
            selectedObject = Selection.activeGameObject;
			selectedScript = null;
			scriptTypeLabel = "Object";

			MonoBehaviour [] scriptsOnObject = selectedObject.GetComponents<coreObjectsBehavior>();
			if (scriptsOnObject.Length == 0)
			{
				scriptsOnObject = selectedObject.GetComponents<coreCharacterBehavior> ();

				if (scriptsOnObject.Length != 0)
				{
					selectedScript = scriptsOnObject [0];
					scriptTypeLabel = "Player";
				} 
			}
			else
			{
				selectedScript = scriptsOnObject [0];
			}
        }
        else
        {
            selectedObject = null;
            selectedScript = null;
        }
        #endregion

        #region Object Menu
        objectFold = EditorGUILayout.InspectorTitlebar(objectFold, selectedObject);
        if (objectFold)
        {
            // Object GUI stuff here
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            if (selectedObject != null)
            {
				selectedObject.name = EditorGUILayout.TextField(scriptTypeLabel + " Name", selectedObject.name, EditorStyles.objectField);
            }
            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Script Menu
        scriptFold = EditorGUILayout.InspectorTitlebar(scriptFold, selectedScript);
        if(scriptFold)
        {
            // Script GUI stuff here
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();

            if(selectedScript == null && selectedObject != null)
            {

				if(GUILayout.Button("Create Script"))
				{
					NewScriptWindow.Init(selectedObject);
				}





//				EditorGUILayout.Space();

				#region Custom Objects Menu
				customScriptsFold = EditorGUILayout.Foldout(customScriptsFold, "Custom Scripts", true);
				if (customScriptsFold)
				{

//						NewScriptWindow.Init(selectedObject);


					GUILayout.BeginVertical(EditorStyles.helpBox);
//					newScriptIdx = EditorGUILayout.Popup("Select Custom Script", 0, customScriptNames);

							
					selectedScriptIdx = EditorGUILayout.Popup( "Select Custom Script", newScriptIdx, customScriptNames);

					if(newScriptIdx != selectedScriptIdx)
					{
						
						//System.Type tmpCustomClass = System.Type.GetType("MonoBehaviour.coreObjectsBehavior."+"jumpBlock", false, true);

						//string tmpFullName = typeof("jumpBlock").FullName;

						//Debug.Log("MonoBehaviour.coreObjectsBehavior+"+customScriptNames[selectedScriptIdx]);
						//ChangeScene();
						//Debug.Log(customScriptNames[selectedScriptIdx]) ;
						//selectedObject.AddComponent( Type.GetType("coreObjectsBehavior."+customScriptNames[selectedScriptIdx])) ;
						//selectedObject.AddComponent(testra
					}
					EditorGUILayout.Space();
					GUILayout.EndVertical();
					
				}
				#endregion


            }
            else if(selectedScript != null)
            {
				GUILayout.BeginHorizontal();
				GUILayout.Label (scriptTypeLabel + " Script");
				if(GUILayout.Button("Open Script"))
                {
					OpenComponentInMonoDevelop(selectedScript, 1);
                }
				GUILayout.EndHorizontal ();
            }

            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

        #region Level Menu
        levelFold = EditorGUILayout.Foldout(levelFold, "Level Selection", true);
        if (levelFold)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            newSceneIdx = EditorGUILayout.Popup("Level Selection", selectedSceneIdx, sceneNames);
            if(newSceneIdx != selectedSceneIdx)
            {
                ChangeScene();
            }
            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Network Menu
        networkFold = EditorGUILayout.Foldout(networkFold, "Network", true);
        if (networkFold)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Network Status ");
            EditorGUILayout.LabelField(networkStatus, networkStatusStyle);
            GUILayout.EndHorizontal();

            RefreshNetworkConnection();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Server ");
            isServer = EditorGUILayout.Toggle(isServer);
            GUILayout.EndHorizontal();
            roomName = EditorGUILayout.TextField("Room Name", roomName, EditorStyles.objectField);

            // Update Network manager settings
            //  OR find our network manager
            RefreshNetworkManager();

            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Creation Menu
        createSettingsFold = EditorGUILayout.Foldout(createSettingsFold, "Creation Settings", true);
        if (createSettingsFold)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enable Physics");
            physicalObject  = EditorGUILayout.Toggle(physicalObject);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Primitive Object Menu
        primitiveObjFold = EditorGUILayout.Foldout(primitiveObjFold, "Primitive Objects", true);
        if (primitiveObjFold)
        {
            // Each Primitive Object requires it's own special method
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (GUILayout.Button("Cube"))
            {
                CreateCube();
            }
            else if (GUILayout.Button("Sphere"))
            {
                CreateSphere();
            }
            else if (GUILayout.Button("Capsule"))
            {
                CreateCapsule();
            }
            else if (GUILayout.Button("Cylinder"))
            {
                CreateCylinder();
            }
            else if (GUILayout.Button("Plane"))
            {
                CreatePlane();
            }
            else if (GUILayout.Button("Quad"))
            {
                CreateQuad();
            }
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Custom Objects Menu
        customObjFold = EditorGUILayout.Foldout(customObjFold, "Custom Objects", true);
        if (customObjFold)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            string[] searchFolders = new string[1];
            searchFolders[0] = "Assets/Resources";
            string[] guids = AssetDatabase.FindAssets("t:Prefab", searchFolders);

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string btnName = path.Split('/').Last().Split('.').First();
                string objName = path.Substring(path.IndexOf('/', path.IndexOf('/') + 1) + 1);
                if (objName.Equals(""))
                {
                    objName = path.Split('/').Last();
                }
                objName = objName.Split('.').First();

                if (btnName.Length > 6 && btnName.Substring(0, 6).Equals("Physic"))
                {
                    goto skipButton; // Essentially 'contiue'
                }
                for (int j = 0; j < ignoreNames.Count; j++)
                {
                    if (btnName == ignoreNames[j])
                    {
                        goto skipButton; // Cannot add 'contiue' properly for this, must be goto
                    }
                }

                if (GUILayout.Button(btnName))
                {
                    CreateObj(objName, path);
                }
                skipButton:;
            }
            GUILayout.EndVertical();
        }
        #endregion

        GUILayout.EndScrollView();
    }

    public static void CreateObj(string objName, string path)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(asset) as GameObject;
        InitShape(obj, objName);
    }

    #region Create Primitive Methods
    [MenuItem("GameObject/3D Object/Cube", false, 0)]
    public static void CreateCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        InitBasicShape(cube, "Cube");
    }

    [MenuItem("GameObject/3D Object/Sphere", false, 1)]
    public static void CreateSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        InitBasicShape(sphere, "Sphere");
    }

    [MenuItem("GameObject/3D Object/Capsule", false, 2)]
    public static void CreateCapsule()
    {
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        InitBasicShape(capsule, "Capsule");
    }

    [MenuItem("GameObject/3D Object/Cylinder", false, 3)]
    public static void CreateCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        InitBasicShape(cylinder, "Cylinder");
    }

    [MenuItem("GameObject/3D Object/Plane", false, 4)]
    public static void CreatePlane()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        InitBasicShape(plane, "Plane");
    }

    [MenuItem("GameObject/3D Object/Quad", false, 5)]
    public static void CreateQuad()
    {
        if(physicalObject)
        {
            Debug.LogWarning("No avaliable collider for [Quad]; skipping creation");
            return;
        }

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        InitBasicShape(quad, "Quad");
        // Quads have a different default spawn behavior
        quad.transform.LookAt(GetViewCenterWorldPos(0));
        quad.transform.Rotate(0, 180, 0);
    }
    #endregion

    public static void InitBasicShape(GameObject obj, string prefabName)
    {
        obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if(defaultMaterial == null)
        {
            defaultMaterial = Resources.Load("SpawnedObjectMaterial") as Material;
        }

        objRenderer.material = defaultMaterial;

        InitShape(obj, prefabName);
    }

    // This method sets an object's shape to center, and attatches the proper script to it
    public static void InitShape(GameObject obj, string prefabName)
    {
        if (Selection.activeTransform != null)
        {
            // Eliminates parent-child relationship, still allows initial position to be the same 
            //obj.transform.parent = Selection.activeTransform;
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

        if (physicalObject)
        {
            if (obj.GetComponent<PhotonRigidbodyView>() == null)
            {
                obj.AddComponent<PhotonRigidbodyView>(); // Track velocities; rigidbody and photon view automatically added
            }
            spawnScript.prefabName = "Physic" + prefabName; // Ensure the proper prefab name for client download
            obj.name = "Physic" + obj.name; // For convience, also change 
        }
        

        UWBPhotonTransformView objTview = obj.GetComponent<UWBPhotonTransformView>();
        if (objTview == null)
            objTview = obj.AddComponent<UWBPhotonTransformView>(); // Attatch this and a PhotonView
        objTview.enableSyncPos(); // These are all off by default
        objTview.enableSyncRot(); // 
        objTview.enableSyncScale(); //
        

        PhotonView objPview = obj.GetComponent<PhotonView>();
        objPview.ObservedComponents = new List<Component>(); // Make sure we're observing this object

        // Attatch physics component
        objPview.ObservedComponents.Add(obj.GetComponent<Transform>());
        if (physicalObject)
            objPview.ObservedComponents.Add(obj.GetComponent<Rigidbody>());

        objPview.synchronization = ViewSynchronization.UnreliableOnChange; // Default
        objPview.ownershipTransfer = OwnershipOption.Takeover;
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
        Vector3 worldPos;
        // When the Scene View is not active, this will fail-- defaults to 0,0,0 in that case
        try
        {
            Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            worldPos = worldRay.GetPoint(5f);
        }
        catch
        {
            Debug.LogWarning("Creating objects without Scene window open will create them at (0, 0, 0)");
            worldPos = new Vector3(0, 0, 0);
        }
        return worldPos;
    }

    // Get a position at the center of the screen, and return that position
    private static Vector3 GetViewCenterWorldPos(float distance)
    {
        Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        Vector3 worldPos = worldRay.GetPoint(distance);

        return worldPos;
    }

	// Open any given script in MonoDevelop (or the default IDE)
	public static void OpenComponentInMonoDevelop(MonoBehaviour component, int gotoLine)
	{

		string[] fileNames = Directory.GetFiles(Application.dataPath, component.GetType().ToString() + ".cs", SearchOption.AllDirectories);

		if (fileNames.Length > 0)
		{
			string relativepath =  "Assets" + fileNames[0].Substring(Application.dataPath.Length);
			AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(relativepath) as TextAsset, 1);
		} 
		else 
		{
			Debug.LogError("Error in [OpenComponentInMonoDevelop()]: File Not Found:" + component.GetType().ToString() + ".cs");
		}
	}
}