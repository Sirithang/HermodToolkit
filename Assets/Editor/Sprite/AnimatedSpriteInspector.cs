using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimatedSprite))]
public class AnimatedSpriteInspector : SpriteInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnimatedSprite spr = target as AnimatedSprite;

        spr.play = EditorGUILayout.Toggle("Play.", spr.play); 
    }
}