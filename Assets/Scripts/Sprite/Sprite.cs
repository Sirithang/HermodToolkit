using UnityEngine;
using System.Collections;

public class Sprite : MonoBehaviour 
{
    public Texture2D spriteSheet;

    public Rect rect = new Rect(0,0, 32, 32);

	// Use this for initialization
    [ContextMenu("Do Start")]
	protected virtual void Start () 
	{
        if(spriteSheet)
            RecreateSprite();
	}


    protected void OnDestroy()
    {
        MaterialDatabase.Unload(spriteSheet);
    }

    

    //=========================

    [ContextMenu("Create Sprite")]
    public void RecreateSprite()
    {
        Vector3[] pos = new Vector3[4];
        Vector3[] norm = new Vector3[4];
        Vector2[] uv = new Vector2[4];

        int[] offsetsX = { 0, 0, 1, 1 };
        int[] offsetsY = { 0, 1, 1, 0 };

        float normalizedX = rect.x / spriteSheet.width;
        float normalizedY = rect.y / spriteSheet.height;
        float normalizedW = rect.width / spriteSheet.width;
        float normalizedH = rect.height / spriteSheet.height;

        for (int i = 0; i < 4; ++i)
        {
            pos[i].Set(offsetsX[i] * rect.width,
                       offsetsY[i] * rect.height,
                               0);

            norm[i].Set(0, 0, -1);

            //color[vertex] = new Color32(255, 255, 255, 255);
            uv[i].Set(normalizedX + offsetsX[i] * normalizedW, normalizedY + offsetsY[i] * normalizedH);
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }

        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        mf.sharedMesh.vertices = pos;
        mf.sharedMesh.normals = norm;
        mf.sharedMesh.uv = uv;
        mf.sharedMesh.triangles = new int[]{0,1,3, 1,2,3};

        mf.sharedMesh.RecalculateBounds();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
        }


        mr.material = MaterialDatabase.Get(spriteSheet);
    }
}