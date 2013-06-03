using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sprite))]
public class SpriteInspector : Editor 
{
    bool _previewShowned = false;


    void OnEnable()
    {

    }

    void OnDisable()
    {

    }


    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        Sprite spr = target as Sprite;

        spr.spriteSheet = EditorGUILayout.ObjectField("SpriteSheet", spr.spriteSheet, typeof(Texture2D), true) as Texture2D;
        spr.rect = EditorGUILayout.RectField("Sprite Rectangle" , spr.rect);

        spr.rect.x = Mathf.RoundToInt(spr.rect.x);
        spr.rect.y = Mathf.RoundToInt(spr.rect.y);
        spr.rect.width = Mathf.RoundToInt(spr.rect.width);
        spr.rect.height = Mathf.RoundToInt(spr.rect.height);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Sprite", GUILayout.MinHeight(40)) && spr.spriteSheet != null)
        {
            spr.RecreateSprite();
        }
        GUILayout.EndHorizontal();

        _previewShowned = EditorGUILayout.Foldout(_previewShowned, "Preview");

        if (_previewShowned && spr.spriteSheet != null)
        {
            Rect normalizedRect = new Rect();

            normalizedRect.x = spr.rect.x / spr.spriteSheet.width;
            normalizedRect.y = spr.rect.y / spr.spriteSheet.height;
            normalizedRect.width = spr.rect.width / spr.spriteSheet.width;
            normalizedRect.height = spr.rect.height / spr.spriteSheet.height;


            Rect r = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(300));
            GUILayout.Box("");
            GUI.DrawTextureWithTexCoords(new Rect(r.x + r.width * 0.5f - spr.rect.width * 0.5f, r.y, spr.rect.width, spr.rect.height), spr.spriteSheet, normalizedRect);
            EditorGUILayout.EndHorizontal();
        }
    }

    //=============================================

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        Sprite spr = target as Sprite;

        if (spr.spriteSheet == null)
            return;

        GUI.DrawTexture(r, spr.spriteSheet, ScaleMode.ScaleToFit);

        float ScaleX = 1.0f;
        float ScaleY = 1.0f;

        float rRatio = r.width / r.height;
        float iRatio = spr.spriteSheet.width / spr.spriteSheet.height;

        if (rRatio < 0 && iRatio < 0)
        {
            ScaleX = Mathf.Min(r.width / spr.spriteSheet.width, 1.0f);
            ScaleY = ScaleX;
        }
        else if (rRatio > 0 && iRatio > 0)
        {
            ScaleY = Mathf.Min(r.height / spr.spriteSheet.height, 1.0f);
            ScaleX = ScaleY;
        }
        else
        {
            if (spr.spriteSheet.width > spr.spriteSheet.height)
            {
                ScaleX = r.width / spr.spriteSheet.width ;
                ScaleY = ScaleX;
            }
            else
            {
                ScaleY = r.height / spr.spriteSheet.height;
                ScaleX = ScaleY;
            }
        }

        // ----- texture creation

        Texture2D texture = new Texture2D((int)spr.rect.width, (int)spr.rect.height);
        texture.filterMode = FilterMode.Point;

        for (int i = 0; i < texture.width; ++i)
        {
            for (int j = 0; j < texture.height; ++j)
            {
                if (i < 2 || j < 2 || i > texture.width - 3 || j > texture.height - 3)
                {
                    texture.SetPixel(i, j, Color.red);
                }
                else
                {
                    texture.SetPixel(i, j, new Color(0, 0, 0, 0));
                }
            }
        }

        texture.Apply();

        GUI.DrawTexture(new Rect(   r.x + r.width * 0.5f - spr.spriteSheet.width * ScaleX * 0.5f + spr.rect.x * ScaleX,
                                    r.y + r.height * 0.5f + spr.spriteSheet.height * ScaleY * 0.5f - (spr.rect.y + spr.rect.height) * ScaleY,
                                    spr.rect.width * ScaleX,
                                    spr.rect.height * ScaleY), texture);

        DestroyImmediate(texture);
    }
}