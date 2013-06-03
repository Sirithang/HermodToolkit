using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Tilemap))]
public class TilemapInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        Tilemap tl = target as Tilemap;

        bool difference = false;

        tl.spriteSheet = EditorGUILayout.ObjectField("Spritesheet : ", tl.spriteSheet, typeof(Texture2D), false) as Texture2D;

        if (tl.width != tl._internalWidth)
        {
            GUI.color = Color.red;
            difference = true;
        }
        else
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }

        tl.width = EditorGUILayout.IntField("width", tl.width);


        if (tl.height != tl._internalHeight)
        {
            GUI.color = Color.red;
            difference = true;
        }
        else
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);

        tl.height = EditorGUILayout.IntField("height", tl.height);

        tl.tileSize = EditorGUILayout.IntField("Tile size", tl.tileSize);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Make Tilemap", GUILayout.MinHeight(40)) && tl.spriteSheet != null)
        {
            tl.BuildMap();
        }

        if (GUILayout.Button("Edit tilemap", GUILayout.MinHeight(40)))
        {
            TilemapEditor e = EditorWindow.GetWindow<TilemapEditor>();
            e.editedTilemap = target as Tilemap;
            e.ShowPopup();
        }
        GUILayout.EndHorizontal();

        if (difference)
        {
            GUI.color = Color.red;
            GUILayout.Label("internal size different from editor size, rebuild tilemap!");
        }

        if (tl.spriteSheet == null)
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("Spritesheet null, can't build map!");
        }
    }
}