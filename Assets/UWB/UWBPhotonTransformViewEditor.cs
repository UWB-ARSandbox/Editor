using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UWBPhotonTransformView))]
public class UWBPhotonTransformViewEditor : Editor
{
    private UWBPhotonTransformView script;
    private Color matColor;
    private Color editorColor;
    private bool skipColor = false;

    private void OnEnable()
    {
        script = (UWBPhotonTransformView) target;

        if(script.gameObject.GetComponent<Renderer>() == null ||
           script.gameObject.GetComponent<Renderer>().sharedMaterial == null)
        {
            skipColor = true;
            return;
        }

        matColor = script.gameObject.GetComponent<Renderer>().sharedMaterial.color;
        editorColor = matColor;
    }

    public override void OnInspectorGUI()
    {
        if (skipColor)
            return;

        Undo.RecordObject(script, "Change color");

        if (editorColor != matColor)
        {
            if (Application.isPlaying)
            {
                script.gameObject.GetComponent<PhotonView>().RPC("ChangeColor", PhotonTargets.All, editorColor.r, editorColor.g, editorColor.b);
            }
            else
            {
                matColor = editorColor;
                Renderer scriptRenderer = script.gameObject.GetComponent<Renderer>();
                Material tempMaterial = new Material(scriptRenderer.sharedMaterial);
                tempMaterial.color = editorColor;
                scriptRenderer.material = tempMaterial;
            }
        }

        editorColor = EditorGUILayout.ColorField("Color ", matColor);
    }
}
