using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraScript))]
public class CameraScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Sync"))
        {

        }

        this.DrawDefaultInspector();
    }
}