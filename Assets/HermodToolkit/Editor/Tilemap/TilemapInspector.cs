using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

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

        if (GUILayout.Button("Save Tilemap"))
        {
            SaveTilemap();
        }

        if (GUILayout.Button("Load Tilemap"))
        {
            ReadTilemap();
        }


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

    protected void SaveTilemap()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save tilemap", "tilemap", "txt", "Enter the nameof the save tilemap");
        Tilemap map = target as Tilemap;

        if (path == "")
        {
            Debug.LogError("Empty path in save the tilemap");
            return;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < map.height; ++i)
        {
            for (int j = 0; j < map.width; ++j)
            {
                sb.Append(map.getIndex(j, i));

                if (j < map.width - 1)
                    sb.Append(";");
            }

            if(i < map.height - 1)
                sb.Append("\n");
        }

        using (StreamWriter outfile = new StreamWriter(path))
        {
            outfile.Write(sb.ToString());
        }
    }

    //------------------------------------------

    protected void ReadTilemap()
    {
        string path = EditorUtility.OpenFilePanel("tilemap", EditorApplication.applicationPath, "txt");
        Tilemap map = target as Tilemap;

        if (path == "")
        {
            return;
        }

        string content = "";
        using (StreamReader outfile = new StreamReader(path))
        {
            content = outfile.ReadToEnd();
        }

        int width = 0;
        int height = 0;

        bool init = false;

        string[] lines = content.Split('\n');
        height = lines.Length;

        int y = 0;
        int x = 0;

        foreach (string line in lines)
        {
            string[] indexes = line.Split(';');
            
            if (!init)
            {
                init = true;
                width = indexes.Length;
                map.width = width;
                map.height = height;
                map.MakeTilemap();
            }

            foreach(string idx in indexes)
            {
                map.setIndex(x, y, System.Convert.ToInt32(idx));
                x++;
            }
            x = 0;
            y++;
        }
    }
}