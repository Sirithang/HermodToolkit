using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Tilemap : MonoBehaviour
{
    public Texture2D spriteSheet;

    public int tileSize = 32;
    public int width = 1;
    public int height = 1;

    [SerializeField]
    [HideInInspector]
    protected int[] indexes;

    [HideInInspector]
    public int _internalWidth;
    [HideInInspector]
    public int _internalHeight;


    void OnDestroy()
    {
        MaterialDatabase.Unload(spriteSheet);
    }

    //**********************************************

    public void BuildMap()
    {
        int[] n_idx = new int[width*height];

        for(int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (indexes != null && i < _internalWidth && j < _internalHeight)
                {
                    n_idx[i * height + j] = indexes[i * _internalHeight + j];
                }
                else
                {
                    n_idx[i * height +j] = -1;
                }
            }
        }

        indexes = n_idx;
        _internalHeight = height;
        _internalWidth = width;
        MakeTilemap();
    }

    //----------

    public void FloodFill(int x, int y, int val)
    {
        if (indexes == null)
            BuildMap();

        int currentVal = indexes[x * height + y];
        List<Vector2> doneCase = new List<Vector2>();
        List<Vector2> toDo = new List<Vector2>();

        toDo.Add(new Vector2(x, y));

        while (toDo.Count > 0)
        {
            Vector2 top = toDo[0];
            toDo.RemoveAt(0);

            if (top.x >= 0 && top.x < width && top.y >= 0 && top.y < height)
            {
                if (!doneCase.Contains(top))
                {
                    int idx = (int)(top.x * _internalHeight + top.y);
                    if (indexes[idx] == currentVal)
                    {
                        indexes[idx] = val;

                        toDo.Add(new Vector2(top.x - 1, top.y));
                        toDo.Add(new Vector2(top.x + 1, top.y));
                        toDo.Add(new Vector2(top.x, top.y + 1));
                        toDo.Add(new Vector2(top.x, top.y - 1));
                    }

                    doneCase.Add(top);
                }
            }
        }
    }

    //----------

    public void setIndex(int x, int y, int val)
    {
        if (indexes == null)
            BuildMap();

        if (x < 0 || y < 0 || x >= _internalWidth || y >= _internalHeight)
            return;

        indexes[x * height + y] = val;
    }

    [ContextMenu("Make tilemap")]
    public void MakeTilemap()
    {
        if (indexes == null || indexes.Length < _internalHeight * _internalWidth)
        {
            BuildMap();
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if(mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        Vector3[] positions = new Vector3[_internalWidth * _internalHeight * 4];
        Vector2[] uv = new Vector2[_internalWidth * _internalHeight * 4];
        Vector3[] normals = new Vector3[_internalWidth * _internalHeight * 4];
        Color32[] color = new Color32[_internalWidth * _internalHeight * 4];


        int[] triangles = new int[_internalWidth * _internalHeight * 6];

        float normalizedW = tileSize /(float)spriteSheet.width;
        float normalizedH = tileSize / (float)spriteSheet.height;

        int nbTileLine = spriteSheet.width / tileSize;

        for (int i = 0; i < _internalWidth; ++i)
        {
            for (int j = 0; j < _internalHeight; ++j)
            {
                int idx = (i * _internalHeight + j);

                for(int k = 0; k < 4; ++k)
                {
                    int[] offsetsX = {0, 0, 1, 1};
                    int[] offsetsY = {0, 1, 1, 0};

                    int vertex = idx * 4 + k;

                    positions[vertex].Set(  i * tileSize + offsetsX[k] * tileSize, 
                                            j * tileSize + offsetsY[k] * tileSize, 
                                            0);
                    int line = indexes[i * height +j] / nbTileLine;
                    int col =  indexes[i * height + j] - line * nbTileLine;

                    
                    normals[vertex].Set(0,0,-1);

                    if (indexes[i * _internalHeight + j] == -1)
                    {
                        color[vertex] = new Color32(0,0,0,0);
                        uv[vertex].Set(0,0);
                    }
                    else
                    {
                        color[vertex] = new Color32(255, 255, 255, 255);
                        uv[vertex].Set(col * normalizedW + offsetsX[k] * normalizedW, line * normalizedH + offsetsY[k] * normalizedH);
                    }
                }

                triangles[idx*6 + 0] = idx * 4;
                triangles[idx*6 + 1] = idx * 4 + 1;
                triangles[idx*6 + 2] = idx * 4 + 3;
                triangles[idx*6 + 3] = idx * 4 + 1;
                triangles[idx*6 + 4] = idx * 4 + 2;
                triangles[idx*6 + 5] = idx * 4 + 3;
            }
        }

        mf.sharedMesh.Clear();
        mf.sharedMesh.vertices = positions;
        mf.sharedMesh.uv = uv;
        mf.sharedMesh.normals = normals;
        mf.sharedMesh.colors32 = color;
        mf.sharedMesh.triangles = triangles;

        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (mr.sharedMaterial != null)
        {
            MaterialDatabase.Unload(mr.sharedMaterial.mainTexture as Texture2D);
        }

        mr.sharedMaterial = MaterialDatabase.Get(spriteSheet);
    }
}