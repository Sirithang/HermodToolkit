using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour 
{
    public bool fixedOnZ = true;

	// Use this for initialization
    [ContextMenu("Do start")]
	void Start () 
    {
        camera.orthographic = true;
        camera.orthographicSize = Screen.height * 0.5f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 20.0f;

        DoSync();
    }
	
	// Update is called once per frame
	void Update () 
    {
        DoSync();
	}

    //======================================

    public void DoSync()
    {
        transform.forward = Vector3.forward;

        Vector3 pos = transform.position;
        pos.z = -2;
        transform.position = pos;

#if UNITY_EDITOR
        camera.orthographic = true;
        camera.orthographicSize = Screen.height * 0.5f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 20.0f;
#endif
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
