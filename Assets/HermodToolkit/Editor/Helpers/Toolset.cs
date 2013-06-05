using UnityEngine;
using System.Collections;

public class Toolset 
{
    static Texture2D _highRes = null;
    static Texture2D _lowRes = null;

    /// <summary>
    /// Very non efficient way to draw a quad. But good enough for the editor.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    static public void DrawGUIQuad(Rect position, Color color)
    {
        if (_highRes == null)
        {
            _highRes = new Texture2D(1024, 1024);
            _lowRes = new Texture2D(64, 64);

            int[] size = { 1024, 64 };
            Texture2D[] tex = { _highRes, _lowRes };

            for (int k = 0; k < 2; ++k)
            {

                for (int i = 0; i < size[k]; ++i)
                {
                    for (int j = 0; j < size[k]; ++j)
                    {
                        if (i < 2 || j < 2 || i > size[k] - 3 || j > size[k] - 3)
                        {
                            tex[k].SetPixel(i, j, color);
                        }
                        else
                        {
                            tex[k].SetPixel(i, j, new Color(0, 0, 0, 0));
                        }
                    }
                }

                tex[k].Apply();
            }
        }


        if (position.width > 256 || position.height > 256)
            GUI.skin.box.normal.background = _highRes;
        else
            GUI.skin.box.normal.background = _lowRes;

        GUI.Box(position, GUIContent.none);
    }
}