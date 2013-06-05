using UnityEngine;
using UnityEditor;
using System.Collections;

public class Sceneview2D : SceneView
{
    static protected Sceneview2D _current;
    static public Sceneview2D current { get { return _current; } }

    protected float _zoom = 1.0f;
    protected bool _lockedSelection = false;

    protected GameObject _obj = null;

    public float zoom
    {
        get { return _zoom; }
        set 
        {
            _zoom = value;

            if (_zoom < 0.2f)
                _zoom = 0.2f;
            else if (_zoom > 5.0f)
                _zoom = 5.0f;
        }
    }

    public bool lockedSelection
    {
        get { return _lockedSelection; }
        set { _lockedSelection = value;}
    }

    //------------------------------

    public override void OnEnable()
    {
        base.OnEnable();
        _current = this;
        onSceneGUIDelegate += SceneFunc;
    }

    public override void OnDisable()
    {
        //base.OnDisable();
        onSceneGUIDelegate -= SceneFunc;
    }

    void OnGUI()
    {
        System.Reflection.MethodInfo dynMethod = typeof(SceneView).GetMethod("OnGUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dynMethod.Invoke(this, new object[] {});

        Color c = GUI.color;
        GUI.color = Color.black;
        EditorGUI.LabelField(new Rect(position.width - 120, position.height - kToolbarHeight, 100, kToolbarHeight), "Zoom : " + (int)(1.0f / _zoom * 100));
        GUI.color = c;
        if (GUI.Button(new Rect(position.width - 50, position.height - kToolbarHeight, 50, kToolbarHeight), "Reset"))
        {
            _zoom = 1.0f;
        }
    }

    void EnforceCameraLimits()
    {
        this.orthographic = true;

        camera.transform.rotation = Quaternion.identity;

        Vector3 pos = camera.transform.position;
        pos.z = -2;
        camera.transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));

        rotation = Quaternion.identity;
        pivot = camera.transform.position - Vector3.forward;
        this.size = 20.0f;

        camera.projectionMatrix = Matrix4x4.Ortho(-position.width * 0.5f * _zoom, position.width * 0.5f * _zoom, -position.height * 0.5f * _zoom, position.height * 0.5f * _zoom, 0.01f, 50.0f);
    }

    static void SceneFunc(SceneView p_scn)
    {
        Sceneview2D scn = p_scn as Sceneview2D;
        if (scn == null)
            return;

        scn.EnforceCameraLimits();

        Event e = Event.current;

        switch (e.type)
        {
            case EventType.ScrollWheel:
                scn.zoom += e.delta.y * 0.05f;
                break;
            default:
                break;
        }
    }

    //--------------------------------------------

    void HandleClickEvent(Event e)
    {

    }

    //============================================

    [MenuItem("Window/Open 2D SceneView", false, 1)]
    static public void OpenView()
    {
        Sceneview2D swin = GetWindow<Sceneview2D>("2DSceneView", typeof(SceneView));
        swin.Show();
    }
}