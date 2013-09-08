using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class PixelLine : MonoBehaviour 
{
    public List<Vector3> points;
    public int size;

	// Use this for initialization
	void Start () 
	{
        RebuildLine();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    //----------------------

    [ContextMenu("Make Line")]
    public void RebuildLine()
    {
        if (points.Count < 2)
            return;

        List<Vector3> pts = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> id = new List<int>();

        Vector3 previous = points[0];

        for (int i = 1; i < points.Count; ++i)
        {
            Vector3 prevToThis = points[i] - previous;
            Vector3 right = Vector3.Cross(prevToThis, Vector3.forward);
            right.Normalize();

            Vector3 toAdd;

            if (i == 1)
            {
                pts.Add(previous);
                pts.Add(previous - right * size);
                uv.Add(Vector2.zero);
                uv.Add(Vector2.zero);
            }

            if (i < points.Count - 1)
            {
                Vector3 currToNext = points[i + 1] - points[i];
                Vector3 nextright = Vector3.Cross(currToNext, Vector3.forward);
                nextright.Normalize();

                toAdd = (right * size + nextright * size) * (0.5f + Mathf.Sin(Vector3.Angle(right, nextright) * Mathf.Deg2Rad)*0.5f);
            }
            else
            {
                toAdd = right * size;
            }

            pts.Add(points[i]);
            pts.Add(points[i] - toAdd);

            uv.Add(Vector2.zero);
            uv.Add(Vector2.zero);

            id.Add((i - 1) * 2);
            id.Add((i - 1) * 2 + 1);
            id.Add(i* 2);

            id.Add((i - 1) * 2 + 1);
            id.Add(i * 2 + 1);
            id.Add(i * 2);

            previous = points[i];
        }

        MeshFilter mf = GetComponent<MeshFilter>();

        DestroyImmediate(mf.sharedMesh);
        mf.sharedMesh = new Mesh();

        mf.sharedMesh.vertices = pts.ToArray();
        mf.sharedMesh.uv = uv.ToArray();
        mf.sharedMesh.triangles = id.ToArray();

        mf.sharedMesh.RecalculateBounds();
    }
}