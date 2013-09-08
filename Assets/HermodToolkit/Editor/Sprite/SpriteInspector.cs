using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sprite))]
public class SpriteInspector : Editor 
{
    bool _previewShowned = false;
    TextureRegionSelector _selector;

    //-----------------------

    public virtual void OnSceneGUI()
    {
        Event e = Event.current;

        Sprite spr = target as Sprite;

        if (e.type == EventType.MouseDrag)
        {
            if (e.modifiers == EventModifiers.Control)
            {
                Vector3 worldMouse = Sceneview2D.current.camera.ScreenToWorldPoint(new Vector3(e.mousePosition.x, Sceneview2D.current.position.height - Sceneview2D.kToolbarHeight - e.mousePosition.y, 1));

                Tilemap[] tl = Tilemap.FindObjectsOfType(typeof(Tilemap)) as Tilemap[];
                if (tl == null)
                    return;

                Vector3 forcedPosition = spr.pos;

                foreach (Tilemap t in tl)
                {
                    Vector2 pos = t.FindCaseAt(worldMouse);
                    if (pos.x >= 0 && pos.y >= 0 && pos.x < t.width && pos.y < t.height)
                    {
                        forcedPosition = t.PositionOfCase((int)pos.x, (int)pos.y);
                    }
                }
                

                spr.transform.position = new Vector3(forcedPosition.x, forcedPosition.y, spr.transform.position.z);

                e.Use();
            }

        }

        spr.pos = spr.transform.position;
        spr.RoundPosition();
    }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
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
        Rect re = EditorGUILayout.RectField("Sprite Rectangle" , spr.rect);

        if (re != spr.rect)
        {
            spr.rect = re;

            if (_selector)
            {
                _selector._currentSelection.x = re.x;
                _selector._currentSelection.y = spr.spriteSheet.height - (re.y + re.height);
                _selector._currentSelection.width = re.width;
                _selector._currentSelection.height = re.height;
            }
        }

        spr.rect.x = Mathf.RoundToInt(spr.rect.x);
        spr.rect.y = Mathf.RoundToInt(spr.rect.y);
        spr.rect.width = Mathf.RoundToInt(spr.rect.width);
        spr.rect.height = Mathf.RoundToInt(spr.rect.height);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Sprite", GUILayout.MinHeight(40)) && spr.spriteSheet != null)
        {
            spr.RecreateSprite();
        }
        if (GUILayout.Button("Pick Rect", GUILayout.MinHeight(40)) && spr.spriteSheet != null)
        {
            if (_selector == null)
            {
                _selector = EditorWindow.GetWindow<TextureRegionSelector>();
                _selector._target = spr.spriteSheet;
                _selector._owner = this;
                _selector._currentSelection.x = re.x;
                _selector._currentSelection.y = spr.spriteSheet.height - (re.y + re.height);
                _selector._currentSelection.width = re.width;
                _selector._currentSelection.height = re.height;
                _selector.Show();
            }
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


        if (_selector && _selector.isOpen)
        {
            float xMin = _selector._currentSelection.xMin;
            float xMax = _selector._currentSelection.xMax;
            float yMin = spr.spriteSheet.height - _selector._currentSelection.yMax;
            float yMax = spr.spriteSheet.height - _selector._currentSelection.yMin;

            spr.rect.x = xMin;
            spr.rect.y = yMin;
            spr.rect.width = xMax - xMin;
            spr.rect.height = yMax - yMin;
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

        Texture2D texture = new Texture2D(Mathf.Max((int)spr.rect.width, 2), Mathf.Max((int)spr.rect.height, 2));
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