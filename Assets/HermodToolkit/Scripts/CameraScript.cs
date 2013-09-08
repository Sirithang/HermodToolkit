using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour 
{
    public bool fixedOnZ = true;
    public float pixelSize = 1;

    public bool fixedResolution = true;
    public Vector2 Window;

    public float TargetPixelSize;
    protected float _currentPixelSizeVelocity;

    protected Vector3 _Target;
    protected Vector3 _TargetVelocity;
    public bool _onTarget = false;
    public Vector3 Target
    {
        get { return _Target; }
        set { _Target = value; _onTarget = true; }
    }
   

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
        //DoSync();
#endif
        if (_onTarget)
        {
            Vector3 pos = Vector3.SmoothDamp(transform.position, _Target, ref _TargetVelocity, 1.0f);
            if(pos == _Target)
                _onTarget = false;

            pos.z = transform.position.z;
            transform.position = pos;
        }

        if (TargetPixelSize != pixelSize)
        {
            pixelSize = Mathf.SmoothDamp(pixelSize, TargetPixelSize, ref _currentPixelSizeVelocity, 0.5f);
            BuildOrtho();
        }
	}

    //======================================

    public void BuildOrtho()
    {
        camera.orthographic = true;

        if (fixedResolution)
        {
            float ratio = Window.x / Window.y;
            float ratioScreen = Screen.width / (float)Screen.height;

            camera.orthographicSize = Mathf.Max(1, Window.y * (ratio / ratioScreen) * 0.5f * pixelSize);
        }
        else
        {
            camera.orthographicSize = Mathf.Max(1, Screen.height * 0.5f);
        }

        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 20.0f;
    }

    public void DoSync()
    {
        transform.forward = Vector3.forward;

        Vector3 pos = transform.position;
        pos.z = -2;
        transform.position = pos;

        BuildOrtho();
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
