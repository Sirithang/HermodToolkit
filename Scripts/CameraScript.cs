using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour 
{
    public bool fixedOnZ = true;
    public int pixelSize = 1;

    public bool fixedResolution = true;
    public Vector2 Window;

	// Use this for initialization
    [ContextMenu("Do start")]
	void Start () 
    {
        DoSync();
    }
	
	// Update is called once per frame
	void Update () 
    {
#if UNITY_EDITOR
        DoSync();
#endif
	}

    //======================================

    public void DoSync()
    {
        transform.forward = Vector3.forward;

        Vector3 pos = transform.position;
        pos.z = -2;
        transform.position = pos;

        camera.orthographic = true;

        if (fixedResolution)
        {
            float ratio = Window.x / Window.y;
            float ratioScreen = Screen.width / (float)Screen.height;

            camera.orthographicSize = Mathf.Max(1, Window.y * (ratio / ratioScreen) * 0.5f);
        }
        else
        {
            camera.orthographicSize = Mathf.Max(1, Screen.height * 0.5f);
        }

        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 20.0f;
    }

    //=================================

    public static Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Vector2 s = GetMainGameViewSize();
        Gizmos.DrawWireCube(transform.position, new Vector3(s.x, s.y, 20));
    }
}
