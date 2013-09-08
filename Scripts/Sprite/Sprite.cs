using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sprite : MonoBehaviour 
{
    static public List<Sprite> allSprite { get { return _allSprite; } }
    static protected List<Sprite> _allSprite = new List<Sprite>();

    public Vector3 pos;

    public Texture2D spriteSheet;

    public Rect rect = new Rect(0,0, 32, 32);


    protected Vector3 _savedPos;

    //-----------------------------

    protected virtual void Awake()
    {
        pos = transform.position;
        if (spriteSheet)
            RecreateSprite();
    }

    protected void OnEnable()
    {
        _allSprite.Add(this);
    }

    protected void OnDisable()
    {
        MaterialDatabase.Unload(spriteSheet);
        _allSprite.Remove(this);
    }

	// Use this for initialization
    [ContextMenu("Do Start")]
	protected virtual void Start () 
	{
        
	}


    protected void OnDestroy()
    {
       
    }

    protected void LateUpdate()
    {
        //RoundPosition();
    }

    public void RoundPosition()
    {
        //transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), transform.position.z);
    }

    //=========================

    public void SaveRoundPos()
    {
        _savedPos = transform.position;
        transform.position = new Vector3(Mathf.Round(_savedPos.x), Mathf.Round(_savedPos.y), transform.position.z);
    }

    public void RestoreSavedPos()
    {
        transform.position = _savedPos;
    }

    public void setSpriteByID(int ID)
    {
        int nbByLine = spriteSheet.width / (int)rect.width;

        int col = ID / nbByLine;
        int x = ID - col * nbByLine;

        rect.x = x * rect.width;
        rect.y = col * rect.height;

        RecreateSprite();
    }

    //=========================

    [ContextMenu("Create Sprite")]
    public void RecreateSprite()
    {
        Vector3[] pos = new Vector3[4];
        Vector3[] norm = new Vector3[4];
        Vector2[] uv = new Vector2[4];

        int[] offsetsX = { 0, 0, 1, 1 };
        int[] offsetsY = { 1, 0, 0, 1 };

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

        DestroyImmediate(mf.sharedMesh);
        mf.sharedMesh = new Mesh();

        mf.sharedMesh.vertices = pos;
        mf.sharedMesh.normals = norm;
        mf.sharedMesh.uv = uv;
        mf.sharedMesh.triangles = new int[]{0,3,1, 1,3,2};

        mf.sharedMesh.RecalculateBounds();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = gameObject.AddComponent<MeshRenderer>();
            mr.sharedMaterial = MaterialDatabase.Get(spriteSheet);
        }

        if (!mr.sharedMaterial)
        {
            mr.sharedMaterial = MaterialDatabase.Get(spriteSheet);
        }
        else if (mr.sharedMaterial.mainTexture != spriteSheet)
        {
            MaterialDatabase.Unload(mr.sharedMaterial.mainTexture as Texture2D);
            mr.sharedMaterial = MaterialDatabase.Get(spriteSheet);
        }
    }

    //-----------------------------------------------

    public bool ScreenRectContains(Vector3 position)
    {
        Vector3 ScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect screenRect = new Rect(ScreenPos.x, ScreenPos.y, rect.width, rect.height);

        return screenRect.Contains(position);
    }
}