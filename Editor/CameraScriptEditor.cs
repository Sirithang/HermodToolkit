using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraScript))]
public class CameraScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
    }
}